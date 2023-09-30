using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Tags.Dtos;

namespace Zenith.Core.Features.Articles.Contracts
{
    public interface IArticleService
    {
        Task<ArticleListDto> GetArticleFeedAsync(ArticleFeedDto feedParameters, string userId);

        Task<ArticleListDto> SearchAsync(ArticleSearchDto searchParameters, string userId);

        Task<ArticleDto?> GetArticleAsync(string slug, string userId);

        Task<ArticleDto> CreateArticleAsync(CreateArticleDto newArticle, string userId, IEnumerable<TagDto> tags);
    }
}
