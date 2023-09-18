using Zenith.SharedKernel;

namespace Zenith.Domain.Entities
{
    public class Comment : EntityBase
    {
        public string Body { get; set; }

        public string UserId { get; set; }

        public virtual ZenithUser User { get; set; }

        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}
