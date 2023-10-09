using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zenith.Core.Infrastructure.Persistence.Infrastructure
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        private const string ConnectionStringName = "Data";

        public TContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                
                .AddUserSecrets<DesignTimeDbContextFactoryBase<TContext>>()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("Data");
            
            Guard.Against.NullOrEmpty(connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

            return CreateNewInstance(optionsBuilder.Options);
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);
    }
}
