using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Common.Mapping;
using Zenith.Common.Responses;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class GetArticlesFeed
    {
        public record Query(ArticleFeedDto FeedParameters): IRequest<Result<PaginatedList<ArticleFeedViewModel>>>;

        public class Handler : IRequestHandler<Query, Result<PaginatedList<ArticleFeedViewModel>>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly ICurrentUserContext _currentUserContext;

            public Handler(IServiceManager serviceManager, IMapper mapper, ILogger<Handler> logger, ICurrentUserContext currentUserContext)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _logger = logger;
                _currentUserContext = currentUserContext;
            }

            public async Task<Result<PaginatedList<ArticleFeedViewModel>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try { 
                    Guard.Against.Null(request.FeedParameters, nameof(request.FeedParameters));

                    var user = _currentUserContext.GetCurrentUserContext();
                    var articleListDto =
                        await _serviceManager.Articles.GetArticleFeedAsync(request.FeedParameters, user.Id);

                   var articleFeedItems = _mapper.Map<IEnumerable<ArticleFeedViewModel>>(articleListDto.Articles);


                    var paginatedArticleFeedItems = articleFeedItems.ToPagedList(articleListDto.TotalCount,
                        request.FeedParameters.PageNumber,
                        request.FeedParameters.PageSize);

                    return Result.Success(paginatedArticleFeedItems);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Error occurred retrieving article feed");
                    return Result.Error(e.Message);
                }
            }
        }
    }
}
