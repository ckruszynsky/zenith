using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Zenith.Common.Exceptions;
using Zenith.Common.Identity;
using Zenith.Common.Responses;
using Zenith.Common.Security;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ZenithUser> _userManager;
        private readonly SignInManager<ZenithUser> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserClaimsPrincipalFactory<ZenithUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;

        public IdentityService(UserManager<ZenithUser> userManager,
            SignInManager<ZenithUser> signInManager,
            IUserClaimsPrincipalFactory<ZenithUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService,
            ITokenGenerator tokenGenerator
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
        }

        public async Task<(Result Result, AuthenticationResponse response)> CreateUserAsync(string username, string firstName, string lastName, string password)
        {
            var user = new ZenithUser
            {
                UserName = username,
                Email = username,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var authResponse = await _tokenGenerator.BuildToken(user.Id, user.UserName);
                return( Result.Success(), authResponse);
            }
            else
            {
                return (Result.Success(), new AuthenticationResponse());
            }
        }

        public async Task<(Result Result, AuthenticationResponse response)> LoginUserAsync(string username, string password)
        {

            var result = await _signInManager.PasswordSignInAsync(username, password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var authResponse = await _tokenGenerator.BuildToken(user.Id, username);
                    return (Result.Success(), authResponse);
                }

                throw new UserNotFoundException();
            }
            else
            {
                return (Result.Success(), new AuthenticationResponse());
            }
        }

        public async Task<bool> IsUserInRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                return claims.Any(x => x.Type.ToLower() == "role" && x.Value == role);
            }
            throw new UserNotFoundException();
        }

        public async Task<bool> Authorize(string userId, string policy)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policy);

            return result.Succeeded;
        }

        public async Task<string> GetUserName(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                return user.Email;
            }
            throw new UserNotFoundException();
        }
    }
}
