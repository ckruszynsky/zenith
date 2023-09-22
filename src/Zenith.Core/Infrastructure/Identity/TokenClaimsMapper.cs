using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Zenith.Common.Exceptions;
using Zenith.Common.Security;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public class TokenClaimsMapper : ITokenClaimsMapper
    {

        public List<Claim> MapToClaims<ZenithUser>(ZenithUser user) where ZenithUser : IdentityUser
        {
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                var claims = new List<Claim>()
                {
                    new Claim("userIdentifier", user.Id),
                    new Claim("email", user.Email)
                };

                return claims;
            }
            throw new UserNotFoundException();
        }
    }
}
