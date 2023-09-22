using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Tests.Infrastructure
{
    public class TokenServiceTest : ITokenService
    {
        public string CreateToken(ZenithUser user)
        {
            return "aSecurityToken";
        }
    }
}
