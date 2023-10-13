using AutoMapper;
using System.Reflection;
using Zenith.Common.Mapping;
using Zenith.Core.Features.Users;
using Zenith.Core.Infrastructure;

namespace Zenith.API.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(configuration => configuration.AddProfile(new MappingProfile(Assembly.GetAssembly(typeof(UserViewModel)))));
            var mapper = mappingConfig.CreateMapper();
            services.AddAutoMapper(ca=> ca.AddProfile(new MappingProfile(Assembly.GetAssembly(typeof(UserViewModel)))));
            return services;
        }
    }
}
