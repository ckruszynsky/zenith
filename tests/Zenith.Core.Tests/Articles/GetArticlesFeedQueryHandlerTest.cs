using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;
using static Zenith.Core.Features.Articles.GetArticlesFeed;

namespace Zenith.Core.Tests.Articles
{
    public class GetArticlesFeedQueryHandlerTest : TestFixture
    {
        private readonly NullLogger<GetArticlesFeed.Handler> _logger;

        public GetArticlesFeedQueryHandlerTest()
        {
            _logger = NullLogger<GetArticlesFeed.Handler>.Instance;
        }
        [Fact]
        public async Task GivenValidRequest_WithPageNumberAndPageSize_ReturnsListArticleFeedItems()
        {
            var pageNumber = 1;
            var pageSize = 1;
            var expectedPageCount = 1;
            var expectedTotalCount = 3;

            var query = new GetArticlesFeed.Query(null,pageNumber, pageSize);

            var handler = new GetArticlesFeed.Handler(ServiceMgr, Mapper, _logger);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.PageNumber.ShouldBe(pageNumber);
            response.PagedInfo.PageSize.ShouldBe(pageSize);
            response.PagedInfo.TotalPages.ShouldBe(expectedPageCount);
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Tags.First().Name.ShouldBe("architecture");
            response.Value.ShouldBeOfType<List<ArticleFeedItem>>();
        }
    }
}
