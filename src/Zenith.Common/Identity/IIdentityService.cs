using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Zenith.Common.Responses;
using Zenith.Common.Security;

namespace Zenith.Common.Identity
{
    public interface IIdentityService
    {

        Task<(Result Result, AuthenticationResponse response)> CreateUserAsync(string username, string firstName, string lastName, string password);
        Task<(Result Result, AuthenticationResponse response)> LoginUserAsync(string username, string password);
        Task<bool> IsUserInRole(string userId, string role);
        Task<bool> Authorize(string userId, string policyName);
        Task<string> GetUserName(string userId);
    }
}
