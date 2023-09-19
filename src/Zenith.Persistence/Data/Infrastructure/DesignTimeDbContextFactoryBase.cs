using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zenith.Persistence.Infrastructure
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        private const string ConnectionStringName = "CONNECTION_STRING";

        public TContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<DesignTimeDbContextFactoryBase<TContext>>()
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration[ConnectionStringName];

            Guard.Against.NullOrEmpty(connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return CreateNewInstance(optionsBuilder.Options);
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);
    }
}
