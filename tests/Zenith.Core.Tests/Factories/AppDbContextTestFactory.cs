using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Tests.Factories
{
    public static class AppDbContextTestFactory
    {
        public static void Destroy(AppDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
