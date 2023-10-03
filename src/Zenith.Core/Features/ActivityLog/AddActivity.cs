using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.ActivityLog.Dtos;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.ActivityLog
{
    public class AddActivity
    {
        public record Notification(AddActivityDto ActivityDto):INotification;

        public class Handler : INotificationHandler<Notification>
        {
            private readonly IServiceManager _serviceManager;
            private readonly ILogger<Handler> _logger;

            public Handler(IServiceManager serviceManager, ILogger<Handler> logger)
            {
                _serviceManager = serviceManager;
                _logger = logger;
            }
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                try
                {
                    await _serviceManager.ActivityLogs.AddActivityAsync(notification.ActivityDto);                    
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error occured while adding activity");
                }
            }
        }

    }
}
