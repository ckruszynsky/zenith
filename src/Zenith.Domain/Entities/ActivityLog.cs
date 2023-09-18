using Zenith.SharedKernel;

namespace Zenith.Domain.Entities
{
    public class ActivityLog : EntityBase
    {
        public string ActivityType { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionId { get; set; }

    }
}
