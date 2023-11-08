using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.API
{
    public class CurrentUserContext: ICurrentUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CurrentUserContext> _logger;

        public CurrentUserContext(IHttpContextAccessor contextAccessor, ILogger<CurrentUserContext> logger)
        {
            _contextAccessor = contextAccessor;            
            _logger = logger;
        }

        public HttpContextUserDto GetCurrentUserContext()
        {
            var currentHttpContext = _contextAccessor.HttpContext;

            if (currentHttpContext?.User != null)
            {
               return new HttpContextUserDto
               {
                   Id = currentHttpContext.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                   UserName = currentHttpContext.User.FindFirstValue(ClaimTypes.UserData) ?? string.Empty,
               };
            }
            
            return new HttpContextUserDto
            {
                Id = string.Empty,
                UserName = string.Empty
            };

        }

        public string GetCurrentUserToken()
        {            
            if(_contextAccessor.HttpContext?.Request.Headers != null) {                
                var authorizationHeader = _contextAccessor.HttpContext.Request.Headers?["Authorization"];
                if (authorizationHeader.HasValue && authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
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
