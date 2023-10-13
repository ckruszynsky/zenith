using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly UserManager<ZenithUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ZenithUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationBehavior(ICurrentUserContext currentUserContext,
            UserManager<ZenithUser> userManager,
            IUserClaimsPrincipalFactory<ZenithUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService)
        {
            _currentUserContext = currentUserContext;
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
            var claimsPrincipal = _currentUserContext.GetCurrentUserContext();
            var currentUser = await _userManager.FindByIdAsync(claimsPrincipal.Id);
            var authorized = false;
            if (authorizeAttributes.Any())
            {
                //must be authenticated
                if (claimsPrincipal.Id == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var authorizationWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
                if (authorizationWithRoles.Any())
                {
                    
                    var claims = await _userManager.GetClaimsAsync(currentUser);
                    foreach (var roles in authorizationWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            if (claims.Any(x => x.Type.ToLower() == "role" && x.Value == role))
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }
                }

                var authorizationWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
                if (authorizationWithPolicies.Any())
                {
                    foreach (var policy in authorizationWithPolicies.Select(a => a.Policy))
                    {
                        if (policy != null)
                        {
                            var principal = await _userClaimsPrincipalFactory.CreateAsync(currentUser);
                            var authorizationResult = await _authorizationService.AuthorizeAsync(principal, policy);
                            if (!authorized)
                            {
                                throw new ForbiddenAccessException();
                            }
                        }
                    }
                }
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }


            }
            //User is Authorized / authorization not required
            return await next();
        }
    }
}
