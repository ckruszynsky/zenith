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
            int pageSize)
        {
            var queryable = _appDbContext.Articles
                            .Include(a => a.Author)
                                .ThenInclude(au => au.Followers)
                            .Include(a => a.ArticleTags)
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
