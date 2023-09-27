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
            int? TagId,
            int PageNumber = 0,
            int PageSize = 10

        ) : IRequest<PagedResult<IEnumerable<ArticleFeedItem>>>;


        public class ArticleTagDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            
        }
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
            public List<ArticleTagDto> Tags { get; set; } 

            public void Mapping(Profile profile)
            {
                profile
                    .CreateMap<Article, ArticleFeedItem>()
                    .ForMember(a => a.Author, opts => opts.MapFrom(a => a.Author.NormalizedUserName))
                    .ForMember(d => d.Tags, o => o.MapFrom(s => s.ArticleTags));

                profile
                    .CreateMap<ArticleTag, ArticleTagDto>()
                    .ForMember(dest => dest.Id, opts => opts.MapFrom(o => o.Id))
                    .ForMember(dest => dest.Name, opts => opts.MapFrom(o => o.Tag.Name));
            }

        }

        public class Handler : IRequestHandler<Query, PagedResult<IEnumerable<ArticleFeedItem>>>
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

            public async Task<PagedResult<IEnumerable<ArticleFeedItem>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var pagedArticleDto =
                    await _serviceManager.Article.GetArticleFeedAsync(request.PageNumber, request.PageSize,
                        request.TagId);

                var pagedInfo = new PagedInfo(request.PageNumber, request.PageSize, pagedArticleDto.Articles.Count(),
                    pagedArticleDto.Count);

                var articleFeedItems = _mapper.Map<IEnumerable<ArticleFeedItem>>(pagedArticleDto.Articles);

                return Result.Success(articleFeedItems).ToPagedResult(pagedInfo);
            }
        }
    }
}
