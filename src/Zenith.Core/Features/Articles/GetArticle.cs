using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public record Query(string slug) : IRequest<Result<ArticleViewModel>>;

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
                var user = await _currentUserContext.GetCurrentUserContext();
                var articleDto = await _serviceManager.Article.GetArticleAsync(request,user.Id);
                
                if (articleDto == null)
                {
                    return Result<ArticleViewModel>.NotFound($"Article with slug {request.slug} not found");
                }

                var articleViewModel = _mapper.Map<ArticleViewModel>(articleDto);
                return Result<ArticleViewModel>.Success(articleViewModel);
            }
        }
    }
}