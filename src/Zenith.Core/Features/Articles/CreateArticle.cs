using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.ViewModels;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class CreateArticle
    {
        public record Command(CreateArticleDto NewArticle) : IRequest<Result<ArticleViewModel>>;

        public class Validator : AbstractValidator<CreateArticleDto>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.Title).NotEmpty();
                RuleFor(cmd => cmd.Body).NotEmpty();
                RuleFor(cmd => cmd.Description).NotEmpty();
                RuleFor(cmd => cmd.TagList).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Command, Result<ArticleViewModel>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<Handler> _logger;
            private readonly IMediator _mediator;

            public Handler(IServiceManager serviceManager, IMapper mapper, ICurrentUserContext currentUserContext,  ILogger<Handler> logger, IMediator mediator)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _currentUserContext = currentUserContext;
                _logger = logger;
                _mediator = mediator;
            }
            
            public async Task<Result<ArticleViewModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try { 
                    Guard.Against.Null(request.NewArticle, nameof(request.NewArticle));                    
                    var user = await _currentUserContext.GetCurrentUserContext();
                    var tags = await _serviceManager.Tags.CreateTagsAsync(request.NewArticle.TagList);
                    var articleDto = await _serviceManager.Articles.CreateArticleAsync(request.NewArticle, user.Id, tags);
                    var articleViewModel = _mapper.Map<ArticleViewModel>(articleDto);
                    await _mediator.Publish(new ActivityLog.AddActivity.Notification(new ActivityLog.Dtos.AddActivityDto
                    {
                        ActivityType = ActivityType.ArticleCreate,                        
                        TransactionType = TransactionType.Article,
                        TransactionId = articleDto.Id.ToString()
                    }), cancellationToken);

                    return Result<ArticleViewModel>.Success(articleViewModel);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Error occurred Creating the article");
                    return Result<ArticleViewModel>.Error(e.Message);
                }
            }
        }        
    }
}
