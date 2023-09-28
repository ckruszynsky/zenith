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

        public async Task<ArticleListDto> GetArticleFeedAsync(GetArticlesFeed.Query request)
        {
            var tagId = request.TagId ?? await GetTagWithMostArticles();

            var queryable = GetArticleQueryable();

            var items = queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));


            var count = await items.CountAsync();
                
             var result  = await items
                .Skip(request.PageNumber * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
             
            return new ArticleListDto
            {
                Articles = _mapper.Map<List<ArticleDto>>(result),
                TotalCount = count
            };
        }

        private async Task<int?> GetTagWithMostArticles()
        {
            int? tagId;
            var tags = _appDbContext
                .ArticleTags
                .Select(a => new {Id = a.TagId})
                .Distinct();

            var tagGroupBy = await tags.GroupBy(p => p.Id)
                .Select(b => new {tagId = b.Key, Count = b.Count()})
                .OrderByDescending(c => c.Count)
                .FirstOrDefaultAsync();

            if (tagGroupBy != null)
            {
                tagId = tagGroupBy.tagId;
            }
            else
            {
                return null;
            }

            return tagId;
        }

        public async Task<ArticleListDto> SearchAsync(SearchArticles.Query request, string userId)
        {
            int totalRecords = 0;
            var noResults = new ArticleListDto
            {
                Articles = new List<ArticleDto>(),
                TotalCount = 0
            };
            
            IQueryable<Article> query = null;
            
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


            if (query == null) return noResults;
            
            totalRecords = query.Count();

            var items = query
                .Skip(request.CurrentPage * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<ArticleDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new ArticleListDto
            {
                Articles = items,
                TotalCount = totalRecords
            };

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
            var query = queryable
                .Where(q => q.Title.ToLowerInvariant().Contains(request.SearchText.ToLowerInvariant()));
            return query;
        }

        private static IQueryable<Article> GetArticlesByAuthor(SearchArticles.Query request, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            var query = queryable
                .Where(q => q.Author.UserName == request.Author);
            return query;
        }

        private async Task<IQueryable<Article>> GetArticlesByTagName(SearchArticles.Query request, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            var tagId = await _appDbContext.Tags.Where(t => t.Name == request.Tag)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            var query = queryable
                .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));
            return query;
        }
    }
}