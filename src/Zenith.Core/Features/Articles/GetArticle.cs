using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Features.Articles.ViewModels;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class GetArticle
    {
        public record Query(string Slug) : IRequest<Result<ArticleViewModel>>;

        public class Handler : IRequestHandler<Query, Result<ArticleViewModel>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly ICurrentUserContext _currentUserContext;

            public Handler(IServiceManager serviceManager, IMapper mapper, ILogger<Handler> logger,
                ICurrentUserContext currentUserContext)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _logger = logger;
                _currentUserContext = currentUserContext;
            }

            public async Task<Result<ArticleViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try { 
                    Guard.Against.NullOrEmpty(request.Slug, nameof(request.Slug));

                    var user = _currentUserContext.GetCurrentUserContext();
                    var articleDto = await _serviceManager.Articles.GetArticleAsync(request.Slug,user.Id);
                
                    if (articleDto == null)
                    {
                        return Result<ArticleViewModel>.NotFound($"Article with slug {request.Slug} not found");
                    }

                    var articleViewModel = _mapper.Map<ArticleViewModel>(articleDto);
                    return Result<ArticleViewModel>.Success(articleViewModel);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred retrieving article");
                    return Result<ArticleViewModel>.Error(e.Message);
                }
            }
        }
    }
}