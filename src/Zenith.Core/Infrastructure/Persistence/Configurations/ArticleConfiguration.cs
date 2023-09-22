using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasOne(a => a.Author)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.AuthorId);
        }
    }
}
