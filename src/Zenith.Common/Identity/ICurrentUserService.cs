using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Common.Identity
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
