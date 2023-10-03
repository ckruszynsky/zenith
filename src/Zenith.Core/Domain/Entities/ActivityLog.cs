using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class ActivityLog : BaseAuditableEntity
    {
        public ActivityType ActivityType { get; set; }
        public TransactionType TransactionType { get; set; }        
        public string TransactionId { get; set; }
    }
}
