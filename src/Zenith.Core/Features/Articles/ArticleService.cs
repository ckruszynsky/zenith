using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Features.Articles.Dtos;
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

        public async Task<ArticleListDto> GetArticleFeedAsync(GetArticlesFeed.Query request, string userId)
        {
            var tagId = request.TagId ?? await GetTagWithMostArticles();

            var queryable = GetArticleQueryable();

            var items = queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));


            var count = await items.CountAsync();
                
             var articleEntities  = await items
                .Skip(request.PageNumber * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var user = GetArticleUserQueryable(userId);

            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articleEntities);
            UpdateArticleFollowing(user, articleDtos, articleEntities);

            return new ArticleListDto
            {
                Articles = articleDtos ?? new List<ArticleDto>(),
                TotalCount = count
            };
        }

        public async Task<ArticleListDto> SearchAsync(SearchArticles.Query request, string userId)
        {
        
            var noResults = new ArticleListDto
            {
                Articles = new List<ArticleDto>(),
                TotalCount = 0
            };
            
            IQueryable<Article>? query = null;
            
            var queryable = GetArticleQueryable();

            if (!string.IsNullOrEmpty(request.Tag))
            {
                query = await GetArticlesByTagName(request, queryable);
            }

            if (!string.IsNullOrEmpty(request.Author))
            {
                query = GetArticlesByAuthor(request, queryable);
            }

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = SearchArticlesByTitle(request, queryable);
            }

            if (request.IncludeOnlyFavorite.HasValue && request.IncludeOnlyFavorite.Value)
            {
                query = GetFavoritedArticles(userId, queryable);
            }

            if (query == null) return noResults;
            
            var totalRecords = query.Count();

            var articleEntities = query
                .Skip(request.CurrentPage * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var user = GetArticleUserQueryable(userId);

            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articleEntities);
            UpdateArticleFollowing(user, articleDtos, articleEntities);

            return new ArticleListDto
            {
                Articles = articleDtos ?? new List<ArticleDto>(),
                TotalCount = totalRecords
            };

        }

      
        public async Task<ArticleDto?> GetArticleAsync(GetArticle.Query request, string userId)
        {
            var user = GetArticleUserQueryable(userId);
            
            IQueryable<Article>? query = GetArticleQueryable();
            
            var articleEntity = query.FirstOrDefault(a => a.Slug == request.slug);
            var articleDto = _mapper.Map<ArticleDto>(articleEntity);

            if (articleEntity == null) return null;
            if (user == null) return articleDto;

            articleDto.Following = IsAuthorFollowedByUser(articleEntity, user);
            articleDto.Favorited = IsArticleFavoritedByUser(articleEntity, user);

            return articleDto;

        }

        private void UpdateArticleFollowing(ZenithUser? user, IEnumerable<ArticleDto> articleDtos, List<Article> articleEntities)
        {
            if (user != null)
            {
                foreach (var articleDto in articleDtos)
                {
                    var entity = articleEntities.Single(a => a.Id == articleDto.Id);
                    articleDto.Following = IsAuthorFollowedByUser(entity, user);
                    articleDto.Favorited = IsArticleFavoritedByUser(entity, user);
                }
            }
        }


        public bool IsArticleFavoritedByUser(Article articleEntity, ZenithUser user)
        {
            return articleEntity.Favorites.Any(f => f.User == user);
        }
        public bool IsAuthorFollowedByUser(Article articleEntity, ZenithUser user)
        {
           return articleEntity.Author.Followers.Any(f => f.UserFollower == user);
                
        }
        
        private IQueryable<Article>? GetFavoritedArticles(string userId, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            IQueryable<Article>? query;
            var user = GetArticleUserQueryable(userId);

            return user != null
                ? queryable
                    .Where(q => q.Favorites.Select(f => f.User).Contains(user))
                : null;
        }

        private ZenithUser? GetArticleUserQueryable(string userId)
        {
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

        private static IQueryable<Article> SearchArticlesByTitle(SearchArticles.Query request, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            return queryable
                .Where(q => q.Title.ToLowerInvariant().Contains(request.SearchText.ToLowerInvariant()));
        }

        private static IQueryable<Article> GetArticlesByAuthor(SearchArticles.Query request, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            return queryable
                .Where(q => q.Author.UserName == request.Author);
        }

        private async Task<IQueryable<Article>> GetArticlesByTagName(SearchArticles.Query request, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            var tagId = await _appDbContext.Tags.Where(t => t.Name == request.Tag)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            return queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));
        }
    }
}