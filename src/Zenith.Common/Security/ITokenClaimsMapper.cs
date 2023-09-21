using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Zenith.Common.Security
{
    public interface ITokenClaimsMapper
    {
        List<Claim> MapToClaims<T>(T user) where T : IdentityUser;
    }
}
