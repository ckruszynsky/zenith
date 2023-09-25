using Microsoft.EntityFrameworkCore;
using Zenith.Core.Domain.Entities;
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

        public async Task<IEnumerable<Article>> GetArticleFeedAsync(
            int pageNumber,
            int pageSize)
        {
            return await _appDbContext.Articles
                .Include(a => a.Author)
                    .ThenInclude(au => au.Followers)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Include(a => a.Favorites)
                .Include(au => au.Comments)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
