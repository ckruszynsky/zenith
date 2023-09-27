using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Tests.Infrastructure;


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
            var pageNumber = 0;
            var pageSize = 1;
            var expectedPageCount = (int)Math.Ceiling((double)1/pageSize);
            var expectedTotalCount = 1;

            var query = new GetArticlesFeed.Query(PageNumber: pageNumber, PageSize: pageSize);

            var handler = new GetArticlesFeed.Handler(ServiceMgr, Mapper, _logger);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.PageNumber.ShouldBe(pageNumber);
            response.PagedInfo.PageSize.ShouldBe(pageSize);
            response.PagedInfo.TotalPages.ShouldBe(expectedPageCount);
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.Any(v => v.Tags.Contains("architecture")).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }
    }
}
