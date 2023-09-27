namespace Zenith.Core.Features.Articles
{
    public interface IArticleService
    {
        Task<PagedArticleDto> GetArticleFeedAsync(
            int pageNumber,
            int pageSize,
            int? tagId);
    }
}
 