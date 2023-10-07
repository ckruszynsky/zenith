using Zenith.Common.Date;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence.Interceptors;
using Zenith.Core.Infrastructure.Services;

namespace Zenith.API.Configuration
{
    public static class CoreConfiguration
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IDateTime,MachineDateTime>();
            services.AddScoped<ICurrentUserContext,CurrentUserContext>();
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            return services;
        }
    }
}
