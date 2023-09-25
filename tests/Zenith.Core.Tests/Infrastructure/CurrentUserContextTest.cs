using Microsoft.AspNetCore.Identity;
using Zenith.Core.Domain.Entities;
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

        public async Task<ZenithUser> GetCurrentUserContext()
        {
            return await _userManager.FindByEmailAsync(TestConstants.TestUserEmail);
        }

        public string GetCurrentUserToken()
        {
            return TestConstants.TokenStringMock;
        }
    }
}
