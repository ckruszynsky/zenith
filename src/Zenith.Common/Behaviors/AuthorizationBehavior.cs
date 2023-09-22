using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Zenith.Common.Exceptions;
using Zenith.Common.Identity;

namespace Zenith.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AuthorizationBehavior(ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
            var authorized = false;
            if (authorizeAttributes.Any())
            {
                //must be authenticated
                if (_currentUserService.UserId == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var authorizationWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
                if (authorizationWithRoles.Any())
                {
                    foreach (var roles in authorizationWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            var isInRole = await _identityService.IsUserInRole(_currentUserService.UserId, role.Trim());
                            if (isInRole)
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
                            authorized = await _identityService.Authorize(_currentUserService.UserId, policy);
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
