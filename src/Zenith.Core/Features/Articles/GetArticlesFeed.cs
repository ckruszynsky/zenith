using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class GetArticlesFeed
    {
        public record Query(
            int PageNumber = 0,
            int PageSize = 10) : IRequest<PagedResult<IEnumerable<ArticleFeedItem>>>;

        public class ArticleFeedItem : IMapFrom<Article>
        {
            public string Slug { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public int FavoritesCount { get; set; }
            public int CommentsCount { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }

            private void Mapping(Profile profile)
            {
                profile
                .CreateMap<Article, GetArticlesFeed.ArticleFeedItem>()
               .ForMember(a => a.Author, opts => opts.MapFrom(a => a.Author.NormalizedUserName));
            }
        }

        public class Handler : IRequestHandler<Query, PagedResult<IEnumerable<ArticleFeedItem>>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(IServiceManager serviceManager, IMapper mapper, ILogger<GetArticlesFeed.Handler> logger)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<PagedResult<IEnumerable<ArticleFeedItem>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pagedArticleDto = await _serviceManager.Article.GetArticleFeedAsync(request.PageNumber, request.PageSize);

                var pageInfo = new PagedInfo(request.PageNumber, request.PageSize, pagedArticleDto.Articles.Count(), pagedArticleDto.Count);

                var result = new Result<IEnumerable<Article>>(pagedArticleDto.Articles)
                    .Map(a => _mapper.Map<IEnumerable<ArticleFeedItem>>(a))
                    .ToPagedResult(pageInfo);

                _logger.LogInformation($"Received request for Article feed with page size: {request.PageSize} and {request.PageNumber} with a result size of: {result.PagedInfo.TotalRecords}");
                return result;
            }
        }

    }
}
