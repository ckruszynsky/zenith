using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class ActivityLog : BaseAuditableEntity
    {
        public required string ActivityType { get; set; }
        public TransactionType TransactionType { get; set; }
        public required string TransactionId { get; set; }

    }
}
