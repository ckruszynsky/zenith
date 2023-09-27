using Microsoft.EntityFrameworkCore;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Articles
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _appDbContext;
        public ArticleService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PagedArticleDto> GetArticleFeedAsync(
            int pageNumber,
            int pageSize,
            int? tagId)
        {

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
                            .Include(a => a.ArticleTags.Where(at=> at.TagId == tagId))
                                .ThenInclude(at => at.Tag)
                            .Include(a => a.Favorites)
                            .Include(au => au.Comments);

            var count = await queryable.CountAsync();

            var items = await queryable
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedArticleDto
            {
                Articles = items,
                Count = count
            };
        }
    }
}
