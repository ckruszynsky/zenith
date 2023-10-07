using Microsoft.AspNetCore.Identity;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.API
{
    public class CurrentUserContext: ICurrentUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ZenithUser> _userManager;
        private readonly ILogger<CurrentUserContext> _logger;

        public CurrentUserContext(IHttpContextAccessor contextAccessor, UserManager<ZenithUser> userManager, ILogger<CurrentUserContext> logger)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ZenithUser> GetCurrentUserContext()
        {
            var currentHttpContext = _contextAccessor.HttpContext;

            if (currentHttpContext?.User != null)
            {
                var currentUser = await _userManager.GetUserAsync(currentHttpContext.User);
                if (currentUser != null) return currentUser;

                _logger.LogError("User was not found in the user manager.");
                throw new NotFoundException("User was not found");
            }


            _logger.LogError("User was not found on the current context");
            throw new NotFoundException("User was not found");

        }

        public string GetCurrentUserToken()
        {            
            if(_contextAccessor.HttpContext?.Request.Headers != null) {                
                var authorizationHeader = _contextAccessor.HttpContext.Request.Headers?["Authorization"];
                if (authorizationHeader.HasValue && authorizationHeader.ToString().StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                {
                    string token = authorizationHeader.ToString().Split(' ')[1];
                    return token;
                }
            }
            _logger.LogError("Token was not found on the current context");
            throw new NotFoundException("Token was not found");
        }
    }
}
