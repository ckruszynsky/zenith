using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.Profile.Dtos;

namespace Zenith.Core.Features.Profile.Contracts
{
    public interface IProfileService
    {
        Task<bool> FollowUser(string username);
        Task<bool> UnfollowUser(string username);
        Task<ProfileDto> GetProfile(string username);
    }
}
