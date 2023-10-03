using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class DeleteArticle
    {
        public record Command(string Slug) : IRequest<Result>;

        public class Validation : AbstractValidator<Command>
        {

            public Validation()
            {
                RuleFor(x => x.Slug).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command,Result>
        {
            private readonly IServiceManager _serviceManager;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<Handler> _logger;
            private readonly IMediator _mediator;

            public Handler(IServiceManager serviceManager, ICurrentUserContext currentUserContext,ILogger<Handler> logger, IMediator mediator)
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
                    var currentUser = await _currentUserContext.GetCurrentUserContext();
                    await _serviceManager.Articles.DeleteArticleAsync(request.Slug, currentUser.Id);

                    await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                    {
                        ActivityType = ActivityType.ArticleDelete,
                        TransactionType = TransactionType.Article,
                        TransactionId = request.Slug
                    }), cancellationToken);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured while deleting article");
                    return Result.Error("Error occured while deleting article");
                }
            }
        }
    }
}

