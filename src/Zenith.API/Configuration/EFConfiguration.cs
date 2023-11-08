using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Zenith.Core.Infrastructure.Persistence;
using Zenith.Core.Infrastructure.Persistence.Interceptors;

namespace Zenith.API.Configuration
{
    public static class EFConfiguration
    {
        public static IServiceCollection AddEFConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
         
            services.AddScoped<SaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Data"),
                    builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            services.AddScoped<AppDbInitializer>();
            

            return services;
        }
    }
}
