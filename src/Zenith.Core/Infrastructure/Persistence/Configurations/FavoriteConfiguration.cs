using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasOne(f => f.Article)
                .WithMany(a => a.Favorites)
                .HasForeignKey(f => f.ArticleId);

            builder.HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);
        }
    }
}
