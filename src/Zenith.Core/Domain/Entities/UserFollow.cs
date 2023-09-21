using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities
{
    public class UserFollow : BaseAuditableEntity
    {
        public string UserFollowerId { get; set; }

        public virtual ZenithUser UserFollower { get; set; }

        public string UserFollowingId { get; set; }

        public virtual ZenithUser UserFollowing { get; set; }
    }
}
