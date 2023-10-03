using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.ActivityLog.Contracts;
using Zenith.Core.Features.ActivityLog.Dtos;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.ActivityLog
{
    public class ActivityLogService: IActivityLogService
    {
        private readonly AppDbContext _context;

        public ActivityLogService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddActivityAsync(AddActivityDto activityDto)
        {
            await _context.ActivityLogs.AddAsync(new Domain.Entities.ActivityLog
            {
                ActivityType = activityDto.ActivityType,
                TransactionType = activityDto.TransactionType,
                TransactionId = activityDto.TransactionId
            });

            await _context.SaveChangesAsync();
        }
    }
}
