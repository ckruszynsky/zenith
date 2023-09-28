using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
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
            //setup 
            var userTestData = Builder<ZenithUser>
                .CreateListOfSize(1)
                .TheFirst(1)
                .With(u => u.UserName = $"test.user{Faker.RandomNumber.Next()}")
                .Build();


            Context.Users.AddRange(userTestData);
            await Context.SaveChangesAsync();

            var tagsTestData = Builder<Tag>
                .CreateListOfSize(3)
                .TheFirst(1)
                .With(t => t.Name = "unit testing")
                .TheNext(1)
                .With(t => t.Name = "dot net development")
                .Build();

            Context.Tags.AddRange(tagsTestData);
            await Context.SaveChangesAsync();

            var articleTestData = Builder<Article>
                .CreateListOfSize(10)
                .TheFirst(6)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> {new() {TagId = tagsTestData[0].Id}})
                .TheNext(4)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();
                
            var expectedTotalCount = 6;
            var pageNumber = 0;
            var pageSize = 2;
            var expectedPageCount = (int)Math.Ceiling((double)expectedTotalCount/pageSize);
            var expectedTagName = "unit testing";
            

            var query = new GetArticlesFeed.Query(PageNumber: pageNumber, PageSize: pageSize);

            var handler = new GetArticlesFeed.Handler(ServiceMgr, Mapper, _logger);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.PageNumber.ShouldBe(pageNumber);
            response.PagedInfo.PageSize.ShouldBe(pageSize);
            response.PagedInfo.TotalPages.ShouldBe(expectedPageCount);
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.Any(v => v.Tags.Contains(expectedTagName)).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }
    }
}
