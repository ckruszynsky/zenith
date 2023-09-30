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
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class SearchArticles
    {
        public record Query(ArticleSearchDto SearchParameters) : IRequest<PagedResult<IEnumerable<ArticleFeedViewModel>>>;


        public class Handler : IRequestHandler<Query, PagedResult<IEnumerable<ArticleFeedViewModel>>>
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
            public async Task<PagedResult<IEnumerable<ArticleFeedViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                try {  
                    Guard.Against.Null(request.SearchParameters, nameof(request.SearchParameters));
                    var currentUser = await _currentUserContext.GetCurrentUserContext();
                    var articleListDto =
                        await _serviceManager.Articles.SearchAsync(request.SearchParameters, currentUser.Id);

                    var pageCount = (int)Math.Ceiling((double)articleListDto.TotalCount / request.SearchParameters.PageSize);
                    var pagedInfo = new PagedInfo(request.SearchParameters.CurrentPage, request.SearchParameters.PageSize, pageCount,
                        articleListDto.TotalCount);

                    var articleFeedItems = _mapper.Map<IEnumerable<ArticleFeedViewModel>>(articleListDto.Articles);

                    return Result.Success(articleFeedItems).ToPagedResult(pagedInfo);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred searching for article");
                    return (PagedResult<IEnumerable<ArticleFeedViewModel>>) Result<IEnumerable<ArticleFeedViewModel>>.Error(e.Message);
                }
            }
        }
    }
}
