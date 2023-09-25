using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Identity
{
    public interface ITokenService
    {
        string CreateToken(ZenithUser user);
    }
}
