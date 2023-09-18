using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Domain.Entities;

namespace Zenith.Persistence.Configurations
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
