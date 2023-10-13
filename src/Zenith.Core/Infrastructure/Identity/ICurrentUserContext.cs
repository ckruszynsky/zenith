using System.Security.Claims;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.Core.Infrastructure.Identity
{
    public interface ICurrentUserContext
    {
        HttpContextUserDto GetCurrentUserContext();

        string GetCurrentUserToken();
    }
}
