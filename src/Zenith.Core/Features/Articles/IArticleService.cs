
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Articles
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetArticleFeedAsync(
            int pageNumber,
            int pageSize);
    }
}
