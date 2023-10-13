using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using System.Reflection;
using Zenith.Core.Behaviors;
using Zenith.Core.Features.Users;

namespace Zenith.API.Configuration
{
    public static class MediatRConfiguration
    {
        public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR((config) => config.RegisterServicesFromAssembly(typeof(CreateUser.Command).Assembly));

            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceTrackingBehavior<,>));
            return services;
        }
    }
}
