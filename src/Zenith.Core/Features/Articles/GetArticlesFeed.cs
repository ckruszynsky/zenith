using Ardalis.Result;
using AutoMapper;
using MediatR;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Articles
{
    public class GetArticlesFeed
    {
        public record Query(
            int PageNumber = 0,
            int PageSize = 10) : IRequest<Result<IEnumerable<ArticleFeedItem>>>;

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

        public class Handler : IRequestHandler<Query, Result<IEnumerable<ArticleFeedItem>>>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IMapper _mapper;

            public Handler(IServiceManager serviceManager, IMapper mapper)
            {
                _serviceManager = serviceManager;
                _mapper = mapper;
            }

            public async Task<Result<IEnumerable<ArticleFeedItem>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var articles = await _serviceManager.Article.GetArticleFeedAsync(request.PageNumber, request.PageSize);
                var feedItems = _mapper.Map<IEnumerable<ArticleFeedItem>>(articles);
                return Result.Success(feedItems);
            }
        }

    }
}
