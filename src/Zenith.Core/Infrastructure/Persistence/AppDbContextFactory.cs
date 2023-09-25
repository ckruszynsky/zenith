using Microsoft.EntityFrameworkCore;
using Zenith.Core.Infrastructure.Persistence.Infrastructure;

namespace Zenith.Core.Infrastructure.Persistence
{
    public class AppDbContextFactory : DesignTimeDbContextFactoryBase<AppDbContext>
    {
        protected override AppDbContext CreateNewInstance(DbContextOptions<AppDbContext> options)
        {
            return new AppDbContext(options);
        }
    }
}
