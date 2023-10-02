using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class DeleteArticleCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<DeleteArticle.Handler> _logger;

        public DeleteArticleCommandHandlerTest()
        {
            _logger = NullLogger<DeleteArticle.Handler>.Instance;
        }

        [Fact]
        public async Task GivenTheRequestIsValid_WhenTheUserIsTheAuthor_ReturnsSuccessAndDeletesArticle()
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

            var currentUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);

            Context.Tags.AddRange(tagsTestData);
            await Context.SaveChangesAsync();

            var articles = Builder<Article>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(a => a.Title = expectedTitle)
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = expectedTitle.ToSlug())
                .With(a => a.AuthorId = currentUser.Id)
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

            //act
            var command = new DeleteArticle.Command(articles[0].Slug);
            var handler = new DeleteArticle.Handler(ServiceMgr,CurrentUserContext,Mapper,_logger);

            var response = await handler.Handle(command, default);

            //assert
            response.IsSuccess.ShouldBeTrue();
            Context.Articles.Count().ShouldBe(1);
            Context.Articles.FirstOrDefault(a => a.Slug == articles[0].Slug).ShouldBeNull();

        }

        [Fact]
        public async Task GivenTheRequestIsValid_WhenTheUserIsNotTheAuthor_ReturnsErrorResult()
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

            var currentUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);

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

            //act
            var command = new DeleteArticle.Command(articles[0].Slug);
            var handler = new DeleteArticle.Handler(ServiceMgr, CurrentUserContext, Mapper, _logger);

            var response = await handler.Handle(command, default);

            //assert
            response.IsSuccess.ShouldBeFalse();

        }

        [Fact]
        public async Task GivenTheRequestIsValid_WhenTheArticleDoesNotExist_ReturnsErrorResult()
        {
          
            //act
            var command = new DeleteArticle.Command("a-test-article");
            var handler = new DeleteArticle.Handler(ServiceMgr, CurrentUserContext, Mapper, _logger);

            var response = await handler.Handle(command, default);

            //assert
            response.IsSuccess.ShouldBeFalse();
        }
    }
}
