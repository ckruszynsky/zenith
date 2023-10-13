using System.Reflection;
using AutoMapper;
using Zenith.Common.Mapping;
using Zenith.Core.Features.Users;

namespace Zenith.Core.Tests.Infrastructure
{
    public static class AutoMapperFactory
    {
        public static IMapper Create()
        {

            var mappingConfig = new MapperConfiguration(configuration => configuration.AddProfile(new MappingProfile(Assembly.GetAssembly(typeof(UserViewModel)))));
            return mappingConfig.CreateMapper();
        }
    }
}
