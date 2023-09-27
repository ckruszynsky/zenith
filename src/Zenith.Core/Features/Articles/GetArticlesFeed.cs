using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class GetArticlesFeed
    {
        public record Query(
            int? TagId =null,
            int PageNumber = 0,
            int PageSize = 10): IRequest<PagedResult<IEnumerable<ArticleFeedViewModel>>>;

        public class Handler : IRequestHandler<Query, PagedResult<IEnumerable<ArticleFeedViewModel>>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(IServiceManager serviceManager, IMapper mapper, ILogger<Handler> logger)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<PagedResult<IEnumerable<ArticleFeedViewModel>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var articleListDto =
                    await _serviceManager.Article.GetArticleFeedAsync(request);

                var pageCount = (int)Math.Ceiling((double)articleListDto.TotalCount / request.PageSize);
                var pagedInfo = new PagedInfo(request.PageNumber, request.PageSize, pageCount,
                    articleListDto.TotalCount);

                var articleFeedItems = _mapper.Map<IEnumerable<ArticleFeedViewModel>>(articleListDto.Articles);

                return Result.Success(articleFeedItems).ToPagedResult(pagedInfo);
            }
        }
    }
}
