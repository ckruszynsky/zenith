using Zenith.Core.Features.Articles.Dtos;

namespace Zenith.Core.Features.Articles.Contracts
{
    public interface IArticleService
    {
        Task<ArticleListDto> GetArticleFeedAsync(GetArticlesFeed.Query request, string userId);

        Task<ArticleListDto> SearchAsync(SearchArticles.Query request, string userId);

        Task<ArticleDto?> GetArticleAsync(GetArticle.Query request, string userId);
            
    }
}
