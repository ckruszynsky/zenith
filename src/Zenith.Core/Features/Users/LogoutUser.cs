using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Features.Users
{
    public class LogoutUser
    {
        public record Command() : IRequest<Result>
        {
            
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ILogger<Handler> _logger;
            private readonly SignInManager<ZenithUser> _signInManager;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly IMediator _mediator;

            public Handler(ILogger<Handler> logger, SignInManager<ZenithUser> signInManager, ICurrentUserContext currentUserContext,
                IMediator mediator)
            {
                _logger = logger;
                _signInManager = signInManager;
                _currentUserContext = currentUserContext;
                _mediator = mediator;
            }
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {                
                var currentUserPrincipal = _currentUserContext.GetCurrentUserContext();
                await _signInManager.SignOutAsync();
                await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                {
                    ActivityType = ActivityType.UserLogout,
                    TransactionType = TransactionType.ZenithUser,
                    TransactionId = currentUserPrincipal.UserName,
                }), cancellationToken);
                return Result.Success();
            }
        }
        
    }
}
