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
using Microsoft.Extensions.Logging.Abstractions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class AddComment
    {
        public record Command(string Slug, AddCommentDto Comment) : IRequest<Result>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Slug).NotEmpty();
                RuleFor(cmd => cmd.Comment).NotNull();
            }

        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceManager _serviceManager;            
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<AddComment.Handler> _logger;
            private readonly IMediator _mediator;

            public Handler(IServiceManager serviceManager,ICurrentUserContext currentUserContext, ILogger<AddComment.Handler> logger, IMediator mediator)
            {
                _serviceManager = serviceManager;                
                _currentUserContext = currentUserContext;
                _logger = logger;
                _mediator = mediator;
            }
            
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _currentUserContext.GetCurrentUserContext();
                    var result = await _serviceManager.Articles.AddCommentAsync(request.Slug, request.Comment, user.Id);

                    await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                    {
                        ActivityType = ActivityType.CommentCreate,
                        TransactionType = TransactionType.Comment,
                        TransactionId = $"{request.Slug}-{user.Id}"
                    }), cancellationToken);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured while adding comment");
                    return Result.Error(ex.Message);
                }
            }
        }
    }
}
