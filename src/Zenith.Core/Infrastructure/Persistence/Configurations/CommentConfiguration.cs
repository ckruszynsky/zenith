using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId);
        }
    }
}
