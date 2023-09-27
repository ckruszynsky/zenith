using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Tests.Infrastructure;
using Zenith.Core.Features.Articles;

namespace Zenith.Core.Tests.Articles
{
    public class SearchArticleQueryHandlerTest : TestFixture
    {
        private readonly NullLogger<SearchArticles.Handler> _logger;

        public SearchArticleQueryHandlerTest()
        {
            _logger = NullLogger<SearchArticles.Handler>.Instance;
        }
        
        [Fact]
        public async Task GivenValidRequest_WithTagAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredByTag()
        {
            var expectedTotalCount = 2;

            var query = new SearchArticles.Query("architecture");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.Any(a=> a.Tags.Contains("architecture")).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }


        [Fact]
        public async Task GivenValidRequest_WithAuthorAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredByAuthor()
        {
            var expectedTotalCount = 1;

            var query = new SearchArticles.Query(Author:"test.user");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe("test.user");
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithAuthorAndTagAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredByTagAndAuthor()
        {
            var expectedTotalCount = 1;

            var query = new SearchArticles.Query(
                Tag:"development",
                Author: "test.user");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe("test.user");
            response.Value.Any(a => a.Tags.Contains("development"));
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithSearchTextAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredBySearchText()
        {
            var expectedTotalCount = 1;
            var searchText = "Vertical";

            var query = new SearchArticles.Query(
                SearchText: searchText);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe("test.user");
            response.Value.First().Title.ToLowerInvariant().Contains(searchText.ToLowerInvariant()).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }
    }
}
