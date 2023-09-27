using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Zenith.Common.Mapping;
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
            var tagId = request.TagId;

            if (!tagId.HasValue)
            {
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
                    throw new ApplicationException("No tags were found to set the default tag for");
                }
                
            }
            var queryable = _appDbContext.Articles
                            .Include(a => a.Author)
                                .ThenInclude(au => au.Followers)
                            .Include(a => a.ArticleTags)
                                .ThenInclude(at => at.Tag)
                            .Include(a => a.Favorites)
                            .Include(au => au.Comments);

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

        public async Task<ArticleListDto> SearchAsync(SearchArticles.Query request, string userId)
        {
            int totalRecords = 0;
            IQueryable<Article> query = null;
            
            var queryable = _appDbContext.Articles
                .Include(a => a.Author)
                .ThenInclude(au => au.Followers)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Include(a => a.Favorites)
                .ThenInclude(a => a.User)
                .Include(au => au.Comments);

            if (!string.IsNullOrEmpty(request.Tag))
            {
                var tagId = await _appDbContext.Tags.Where(t => t.Name == request.Tag)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                query = queryable
                    .Where(q => q.ArticleTags.Any(k => k.TagId == tagId));
            }

            if (!string.IsNullOrEmpty(request.Author))
            {
                query = queryable
                    .Where(q => q.Author.UserName == request.Author);
            }

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = queryable
                    .Where(q => q.Title.ToLowerInvariant().Contains(request.SearchText.ToLowerInvariant()));
            }


            if (query == null) throw new ApplicationException("Error executing query");
            
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
    }
}