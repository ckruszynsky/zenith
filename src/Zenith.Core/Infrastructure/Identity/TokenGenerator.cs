using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Zenith.Common.Date;
using Zenith.Common.Exceptions;
using Zenith.Common.Responses;
using Zenith.Common.Security;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<ZenithUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenClaimsMapper _tokenClaimsMapper;
        private readonly IDateTime _dateTime;

        public TokenGenerator(UserManager<ZenithUser> userManager,
              IConfiguration configuration,
              ITokenClaimsMapper tokenClaimsMapper,
              IDateTime dateTime)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenClaimsMapper = tokenClaimsMapper;
            _dateTime = dateTime;
        }

        public async Task<AuthenticationResponse> BuildToken(string id, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                var claimsDB = await _userManager.GetClaimsAsync(user);
                var claims = _tokenClaimsMapper.MapToClaims(user);

                claims.AddRange(claimsDB);

                var configKey = _configuration["Jwt:Key"];
                if (configKey != null)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var expiration = _dateTime.Now.AddYears(1);

                    var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                        expires: expiration, signingCredentials: creds);

                    return new AuthenticationResponse()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = expiration
                    };
                }
            }
            throw new UserNotFoundException();
        }
    }
}
