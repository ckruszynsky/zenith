using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasOne(uf => uf.UserFollower)
                .WithMany(uf => uf.Following)
                .HasForeignKey(uf => uf.UserFollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(uf => uf.UserFollowing)
                .WithMany(uf => uf.Followers)
                .HasForeignKey(uf => uf.UserFollowingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
