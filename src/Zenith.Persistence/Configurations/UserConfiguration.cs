using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Domain.Entities;

namespace Zenith.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ZenithUser>
    {
        public void Configure(EntityTypeBuilder<ZenithUser> builder)
        {
            builder.HasMany(cu => cu.Articles)
                .WithOne(a => a.Author)
                .HasForeignKey(cu => cu.AuthorId);

            builder.Property(cu => cu.Bio)
                .HasDefaultValue();

            builder.Property(cu => cu.Email)
                .IsRequired();
        }
    }
}
