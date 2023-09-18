namespace Zenith.Domain.Entities
{
    public class UserFollow : BaseEntity
    {
        public string UserFollowerId { get; set; }

        public virtual ZenithUser UserFollower { get; set; }

        public string UserFollowingId { get; set; }

        public virtual ZenithUser UserFollowing { get; set; }
    }
}
