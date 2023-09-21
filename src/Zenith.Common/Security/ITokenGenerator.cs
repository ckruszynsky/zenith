using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Responses;

namespace Zenith.Common.Security
{
    public interface ITokenGenerator
    {
        Task<AuthenticationResponse> BuildToken(string id, string userName);
    }
}
