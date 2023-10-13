using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zenith.Common.Date;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IDateTime _dateTime;

        public TokenService(IConfiguration configuration, IDateTime dateTime)
        {
            _configuration = configuration;
            _dateTime = dateTime;
        }

        public string CreateToken(ZenithUser user)
        {
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]);
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];

            var securityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: GetDefaultClaims(user, issuer, audience),
                notBefore: _dateTime.Now,
                expires: _dateTime.Now.AddMinutes(60),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        private static ClaimsIdentity BuildUserBasedClaims(ZenithUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.UserData, user.UserName),
                new Claim("username", user.UserName),
            };

            return new ClaimsIdentity(claims);
        }

        private static IEnumerable<Claim> GetDefaultClaims(ZenithUser user, string issuer, string audience)
        {
            return new[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(ClaimTypes.UserData, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),                
            };
        }
    }
}
