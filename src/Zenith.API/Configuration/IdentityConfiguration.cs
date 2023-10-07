using Microsoft.AspNetCore.Identity;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.API.Configuration
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ZenithUser,IdentityRole>(options=>
            {
                options.Password.RequireDigit = configuration.GetValue<bool>("Identity:Password:RequireDigit");
                options.Password.RequireLowercase = configuration.GetValue<bool>("Identity:Password:RequireLowercase");
                options.Password.RequireUppercase = configuration.GetValue<bool>("Identity:Password:RequireUppercase");
                options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
            }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
