using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenith.Domain.Entities;

namespace Zenith.Persistence.Configurations
{
    public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
    {
        public void Configure(EntityTypeBuilder<ArticleTag> builder)
        {
            builder.HasOne(a => a.Article)
                .WithMany(at => at.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            builder.HasOne(a => a.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(at => at.TagId);
        }
    }
}
