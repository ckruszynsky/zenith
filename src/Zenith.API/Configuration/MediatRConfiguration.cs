using MediatR;
using MediatR.Pipeline;
using System.Reflection;
using Zenith.Core.Behaviors;

namespace Zenith.API.Configuration
{
    public static class MediatRConfiguration
    {
        public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddMediatR((config) => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceTrackingBehavior<,>));
            return services;
        }
    }
}
