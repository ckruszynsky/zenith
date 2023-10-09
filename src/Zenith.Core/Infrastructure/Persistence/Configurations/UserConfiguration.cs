using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ZenithUser>
    {
        public void Configure(EntityTypeBuilder<ZenithUser> builder)
        {
            builder.HasMany(cu => cu.Articles)
                .WithOne(a => a.Author)
                .HasForeignKey(cu => cu.AuthorId);

            builder.Property(cu => cu.Email)
                .IsRequired();
        }
    }
}
