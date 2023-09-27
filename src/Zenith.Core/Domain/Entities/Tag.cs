using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class Tag : BaseAuditableEntity
    {
        public Tag()
        {
            ArticleTags = new List<ArticleTag>();
        }

        public string Name { get; set; }
        public virtual ICollection<ArticleTag> ArticleTags { get; }
    }
}
