using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class ArticleTag : BaseAuditableEntity
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }


    }
}
