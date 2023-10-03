using System.Collections;
using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Zenith.Common.Exceptions;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Tags.Dtos;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Articles
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ArticleService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ArticleListDto> GetArticleFeedAsync(ArticleFeedDto feedParameters, string userId)
        {
            Guard.Against.Null(feedParameters, nameof(feedParameters));
            Guard.Against.NullOrEmpty(userId, nameof(userId));

            var tagId = feedParameters.TagId ?? await GetTagWithMostArticles();

            var queryable = GetArticleQueryable();

            var items = queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));


            var count = await items.CountAsync();
                
             var articleEntities  = await items
                .Skip(feedParameters.PageNumber * feedParameters.PageSize)
                .Take(feedParameters.PageSize)
                .ToListAsync();

            var user = GetArticleUserQueryable(userId);

            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articleEntities);
            articleDtos.UpdateArticleFollowing(user, articleEntities);            
            return new ArticleListDto
            {
                Articles = articleDtos ?? new List<ArticleDto>(),
                TotalCount = count
            };
        }

        public async Task<ArticleListDto> SearchAsync(ArticleSearchDto searchParameters, string userId)
        {
            Guard.Against.Null(searchParameters, nameof(searchParameters));
            Guard.Against.NullOrEmpty(userId, nameof(userId));

            var noResults = new ArticleListDto
            {
                Articles = new List<ArticleDto>(),
                TotalCount = 0
            };
            
            IQueryable<Article>? query = null;
            
            var queryable = GetArticleQueryable();

            if (!string.IsNullOrEmpty(searchParameters.Tag))
            {
                query = await GetArticlesByTagName(searchParameters, queryable);
            }

            if (!string.IsNullOrEmpty(searchParameters.Author))
            {
                query = searchParameters.GetArticlesByAuthor(queryable);                
            }

            if (!string.IsNullOrEmpty(searchParameters.SearchText))
            {
                query = searchParameters.SearchArticlesByTitle(queryable);
            }

            if (searchParameters.IncludeOnlyFavorites.HasValue && searchParameters.IncludeOnlyFavorites.Value)
            {
                query = GetFavoritedArticles(userId, queryable);
            }

            if (query == null) return noResults;
            
            var totalRecords = query.Count();

            var articleEntities = query
                .Skip(searchParameters.CurrentPage * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .ToList();

            var user = GetArticleUserQueryable(userId);

            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articleEntities);
            
            if(!articleDtos.Any()) return noResults;

            articleDtos.UpdateArticleFollowing(user, articleEntities);
            
            return new ArticleListDto
            {
                Articles = articleDtos ?? new List<ArticleDto>(),
                TotalCount = totalRecords
            };

        }
      
        public async Task<ArticleDto?> GetArticleAsync(string slug, string userId)
        {
            Guard.Against.NullOrEmpty(slug, nameof(slug));
            Guard.Against.NullOrEmpty(userId, nameof(userId));

            var user = GetArticleUserQueryable(userId);
            
            IQueryable<Article>? query = GetArticleQueryable();
            
            var articleEntity = await query.FirstOrDefaultAsync(a => string.Equals(a.Slug,slug,StringComparison.OrdinalIgnoreCase));
            var articleDto = _mapper.Map<ArticleDto>(articleEntity);

            if (articleEntity == null) return null;
            if (user == null) return articleDto;

            articleDto.Following = articleEntity.IsAuthorFollowedByUser(user);
            articleDto.Favorited = articleEntity.IsArticleFavoritedByUser(user);

            return articleDto;

        }

        public async Task<ArticleDto> UpdateArticleAsync(string slug, UpdateArticleDto updateArticleDto,IEnumerable<TagDto>? tags, string userId)
        {
            var query = GetArticleQueryable();
            var articleToUpdate = await query
                .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug, StringComparison.CurrentCultureIgnoreCase)
                && a.AuthorId == userId);

            if (articleToUpdate == null)
            {
                throw new Common.Exceptions.NotFoundException($"Article with slug {slug} not found");
            }
            
            if (updateArticleDto.Title != articleToUpdate.Title)
            {
                articleToUpdate.Title = updateArticleDto.Title;
                articleToUpdate.Slug = updateArticleDto.Title.ToSlug();
            }

            articleToUpdate.Description = updateArticleDto.Description;
            articleToUpdate.Body = updateArticleDto.Body;

            if(tags.Any())
            {                
                articleToUpdate.ArticleTags.Clear();
                foreach (var tag in tags )
                {               
                    articleToUpdate.ArticleTags.Add(new ArticleTag
                    {
                        Article = articleToUpdate,
                        TagId = tag.Id
                    });
                }
            }


            await _appDbContext.SaveChangesAsync();
            
            var articleDto = _mapper.Map<ArticleDto>(articleToUpdate);
            return articleDto;
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto newArticle, string userId, IEnumerable<TagDto> tags)
        {
            Guard.Against.Null(newArticle, nameof(newArticle));
            Guard.Against.NullOrEmpty(userId, nameof(userId));
            var tagDtos = tags.ToList();
            Guard.Against.NullOrEmpty(tagDtos, nameof(tags));


            var articleEntity = _mapper.Map<Article>(newArticle);            
            articleEntity.AuthorId = userId;
            articleEntity.Slug = newArticle.Title.ToSlug();

            var existingArticle = await _appDbContext.Articles.FirstOrDefaultAsync(a => a.Slug == articleEntity.Slug);
            if (existingArticle != null)
            {
                throw new EntityExistsException($"Article with slug {articleEntity.Slug} already exists");
            }

            foreach (var tag in tagDtos)
            {
                articleEntity.ArticleTags.Add(new ArticleTag
                {
                    Article = articleEntity,
                    TagId = tag.Id
                });
            }

            await _appDbContext.Articles.AddAsync(articleEntity);
            await _appDbContext.SaveChangesAsync();

            var articleDto = _mapper.Map<ArticleDto>(articleEntity);

            return articleDto;
        }
           
        public async Task<bool> DeleteArticleAsync(string slug, string userId)
        {
            Guard.Against.NullOrEmpty(slug, nameof(slug));
            Guard.Against.NullOrEmpty(userId, nameof(userId));

            var query = GetArticleQueryable();
            var articleToDelete = await query
                .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug, StringComparison.CurrentCultureIgnoreCase)
                                                         && a.AuthorId == userId);

            if (articleToDelete == null)
            {
                throw new Common.Exceptions.NotFoundException($"Article with slug {slug} not found");
            }

            _appDbContext.Articles.Remove(articleToDelete);
            await _appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddCommentAsync(string slug, AddCommentDto comment, string userId)
        {
                var query = GetArticleQueryable();
                var article = await query
                    .FirstOrDefaultAsync(a => string.Equals(a.Slug, slug, StringComparison.CurrentCultureIgnoreCase));
                
                if(article == null)
                {
                    throw new Common.Exceptions.NotFoundException($"Article with slug {slug} not found");
                }

                var commentEntity = new Comment
                {
                    Body = comment.Body,
                    UserId = userId,
                    ArticleId = article.Id
                };

                article.Comments.Add(commentEntity);
                await _appDbContext.SaveChangesAsync();
                return true;          
        }

        private IQueryable<Article>? GetFavoritedArticles(string userId, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            Guard.Against.NullOrEmpty(userId, nameof(userId));
            Guard.Against.Null(queryable, nameof(queryable));

            IQueryable<Article>? query;
            var user = GetArticleUserQueryable(userId);

            return user != null
                ? queryable
                    .Where(q => q.Favorites.Select(f => f.User).Contains(user))
                : null;
        }

        private ZenithUser? GetArticleUserQueryable(string userId)
        {
            Guard.Against.NullOrEmpty(userId, nameof(userId));
            return _appDbContext.Users
                .Include(u=> u.Followers)
                .Include(u => u.Favorites)
                .FirstOrDefault(u => u.Id == userId);
        }
        
        private async Task<int?> GetTagWithMostArticles()
        {
            int? tagId;
            var tags = _appDbContext
                .ArticleTags
                .Select(a => new { Id = a.TagId })
                .Distinct();

            var tagGroupBy = await tags.GroupBy(p => p.Id)
                .Select(b => new { tagId = b.Key, Count = b.Count() })
                .OrderByDescending(c => c.Count)
                .FirstOrDefaultAsync();

            if (tagGroupBy == null) return null;
            tagId = tagGroupBy.tagId;

            return tagId;

        }

        private IIncludableQueryable<Article, ICollection<Comment>> GetArticleQueryable()
        {
            return _appDbContext.Articles
                .Include(a => a.Author)
                .ThenInclude(au => au.Followers)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Include(a => a.Favorites)
                .ThenInclude(a => a.User)
                .Include(au => au.Comments);
        }
     
        private async Task<IQueryable<Article>> GetArticlesByTagName(ArticleSearchDto searchParameters, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            Guard.Against.Null(searchParameters, nameof(searchParameters));
            Guard.Against.NullOrEmpty(searchParameters.Tag, nameof(searchParameters.Tag));

            var tagId = await _appDbContext.Tags.Where(t => t.Name == searchParameters.Tag)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            return queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));
        }
    }
}