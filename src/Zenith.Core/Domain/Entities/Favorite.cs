using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class Favorite : BaseAuditableEntity
    {
        public string UserId { get; set; }

        public virtual ZenithUser User { get; set; }

        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}
