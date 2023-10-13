using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class UnfavoriteArticle
    {
        public record Command(string Slug) : IRequest<Result>;

        public class  Validator:AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Slug).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceManager _service;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<Handler> _logger;
            private readonly IMediator _mediator;

            public Handler(IServiceManager service, ICurrentUserContext currentUserContext, ILogger<Handler> logger, IMediator mediator)
            {
                _service = service;
                _currentUserContext = currentUserContext;
                _logger = logger;
                _mediator = mediator;
            }
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _currentUserContext.GetCurrentUserContext();
                    await _service.Articles.UnfavoriteArticleAsync(request.Slug, user.Id);
                    await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                    {
                        ActivityType = ActivityType.UserUnfavorite,
                        TransactionType = TransactionType.Favorite,
                        TransactionId = $"{request.Slug}-{user.Id}"
                    }), cancellationToken);
                    return Result.Success();

                }catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured while unfavoriting article");
                    return Result.Error(ex.Message);
                }
            }
        }
        
    }
}
