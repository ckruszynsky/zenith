using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.Property(a => a.ActivityType)
                .HasConversion(new EnumToStringConverter<ActivityType>());

            builder.Property(a => a.TransactionType)
                .HasConversion(new EnumToStringConverter<TransactionType>());
        }
    }
}
