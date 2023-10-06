using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Profile
{
    public class UnfollowUser
    {
        public record Command(string Username) : IRequest<Result>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IMapper _mapper;
            private readonly IServiceManager _serviceManager;
            private readonly ILogger<Handler> _logger;
            private readonly IMediator _mediator;

            public Handler(IMapper mapper, IServiceManager serviceManager, ILogger<Handler> logger, IMediator mediator)
            {
                _mapper = mapper;
                _serviceManager = serviceManager;
                _logger = logger;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await _serviceManager.Profiles.UnfollowUser(request.Username);

                    await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                    {
                        ActivityType = ActivityType.UserUnfollow,
                        TransactionType = TransactionType.UserFollow,
                        TransactionId = request.Username
                    }), cancellationToken);

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error un-following user");
                    return Result.Error("Error un-following user");
                }
            }
        }
    }
}