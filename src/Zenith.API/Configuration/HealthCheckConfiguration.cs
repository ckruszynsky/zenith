using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.API.Configuration
{
    public static class HealthCheckConfiguration
    {
        public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("Data") ?? throw new ApplicationException("Cannot read DB Configuration"),
                failureStatus: HealthStatus.Degraded,               
                name: "ZenithDB-check", 
                tags: new string[] { "ZenithDB" })
                .AddDbContextCheck<AppDbContext>(name: "ZenithDBContext-check",
                customTestQuery: async (context, token) => await context.ActivityLogs.AsNoTracking().ToListAsync(token) != null);
            
            return services;
        }
    }
}
