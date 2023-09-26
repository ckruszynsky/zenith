using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Articles
{
    public class PagedArticleDto
    {
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
        public int Count { get; set; } = 0;
    }
}
