using Ardalis.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.ViewModels;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.ServiceManger;
using AutoMapper;
using Zenith.Core.Infrastructure.Identity;
using Microsoft.Extensions.Logging;

namespace Zenith.Core.Features.Articles
{
    public class UpdateArticle
    {
        public record Command(string Slug, UpdateArticleDto UpdatedArticle):IRequest<Result<ArticleViewModel>>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Slug).NotEmpty();
                RuleFor(x => x.UpdatedArticle).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, Result<ArticleViewModel>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ICurrentUserContext _currentUserContext;
            private readonly ILogger<Handler> _logger;

            public Handler(IServiceManager serviceManager, IMapper mapper, ICurrentUserContext currentUserContext, ILogger<Handler> logger)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _currentUserContext = currentUserContext;
                _logger = logger;
            }

            public async Task<Result<ArticleViewModel>> Handle(Command request, CancellationToken cancellationToken)
            {
               try { 
                    var currentUser = _currentUserContext.GetCurrentUserContext();
                    var tags = await _serviceManager.Tags.CreateTagsAsync(request.UpdatedArticle.Tags);
                    var articleDto = await _serviceManager.Articles.UpdateArticleAsync(request.Slug, request.UpdatedArticle,tags, currentUser.Id);
                    var articleViewModel = _mapper.Map<ArticleViewModel>(articleDto);
                    return Result<ArticleViewModel>.Success(articleViewModel);
               }
               catch (Exception ex)
               {
                   _logger.LogError(ex, $"An error occurred updating the article: {request.Slug}");
                   return Result<ArticleViewModel>.Error(ex.Message);
               }   
            }
        }


    }
}
