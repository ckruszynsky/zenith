using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users.Dtos;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Tests.Infrastructure
{
    public class CurrentUserContextTest : ICurrentUserContext
    {
        private readonly UserManager<ZenithUser> _userManager;

        public CurrentUserContextTest(UserManager<ZenithUser> userManager)
        {
            _userManager = userManager;
        }

        public HttpContextUserDto GetCurrentUserContext()
        {
            var user = _userManager.Users.First(u=> u.UserName == TestConstants.TestUserName);
            return new HttpContextUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
            };
        }

        public string GetCurrentUserToken()
        {
            return TestConstants.TokenStringMock;
        }
    }
}
