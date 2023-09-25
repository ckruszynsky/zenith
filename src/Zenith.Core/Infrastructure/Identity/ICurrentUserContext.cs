using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public interface ICurrentUserContext
    {
        Task<ZenithUser> GetCurrentUserContext();

        string GetCurrentUserToken();
    }
}
