using AutoMapper;
using System.Reflection;
using Zenith.Common.Mapping;
using Zenith.Core.Features.Users;

namespace Zenith.Core.Tests.Factories
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
