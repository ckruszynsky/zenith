using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Common.Responses;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class SearchArticles
    {
        public record Query(ArticleSearchDto SearchParameters) : IRequest<Result<PaginatedList<ArticleFeedViewModel>>>;


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
            public async Task<Result<PaginatedList<ArticleFeedViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                try {  
                    Guard.Against.Null(request.SearchParameters, nameof(request.SearchParameters));
                    var currentUser = _currentUserContext.GetCurrentUserContext();
                    var articleListDto =
                        await _serviceManager.Articles.SearchAsync(request.SearchParameters, currentUser.Id);              

                    var articleFeedItems = _mapper.Map<IEnumerable<ArticleFeedViewModel>>(articleListDto.Articles);
                    var paginatedList = articleFeedItems.ToPagedList(articleListDto.TotalCount,
                            request.SearchParameters.CurrentPage,
                            request.SearchParameters.PageSize);                                               

                    return Result.Success(paginatedList);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred searching for article");
                    return Result.Error(e.Message);
                }
            }
        }
    }
}
