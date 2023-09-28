using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class GetArticleQueryHandlerTest:TestFixture
    {
        private readonly NullLogger<GetArticle.Handler> _logger;

        public GetArticleQueryHandlerTest()
        {
            _logger = NullLogger<GetArticle.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleExists_ReturnsArticleViewModel()
        {
            var expectedTitle = "Test Article";

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
            
            var articles = Builder<Article>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(a => a.Title = expectedTitle)
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = expectedTitle.ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheRest()
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articles);
            await Context.SaveChangesAsync();

            var request = new GetArticle.Query(slug:"test-article");
            var handler = new GetArticle.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);
            var response = await handler.Handle(request, CancellationToken.None);

            response.Value.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Value.Title.ShouldBe(expectedTitle);
            response.Value.Following.ShouldBeFalse();
            response.Value.Favorited.ShouldBeFalse();
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleDoesNotExist_ReturnsErrorResult()
        {
            var expectedTitle = "Test Article";
            var request = new GetArticle.Query(slug: "test-article");
            var handler = new GetArticle.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);
            var response = await handler.Handle(request, CancellationToken.None);

            response.Value.ShouldBeNull();
            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldNotBeEmpty();
            response.Errors.Count().ShouldBe(1);
            response.Errors.First().ShouldBe("Article with slug test-article not found");
        }
    }
}
