using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class Comment : BaseAuditableEntity
    {
        public string Body { get; set; }

        public string UserId { get; set; }

        public virtual ZenithUser User { get; set; }

        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}
