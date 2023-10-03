using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.ActivityLog.Dtos;

namespace Zenith.Core.Features.ActivityLog.Contracts
{
    public interface IActivityLogService
    {
        Task AddActivityAsync(AddActivityDto activityDto);
    }
}
