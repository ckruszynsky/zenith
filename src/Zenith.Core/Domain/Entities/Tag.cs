using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class Tag : BaseAuditableEntity
    {
        public Tag()
        {
            ArticleTags = new List<ArticleTag>();
        }

        public string Description { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; }
    }
}
