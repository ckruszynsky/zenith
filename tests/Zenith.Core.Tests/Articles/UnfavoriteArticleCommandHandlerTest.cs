using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Tests.Articles
{
    public class UnfavoriteArticleCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<UnfavoriteArticle.Handler> _logger;

        public UnfavoriteArticleCommandHandlerTest()
        {
            _logger = NullLogger<UnfavoriteArticle.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleExistsAndUserHasFavored_ReturnsSuccessAndRemovesFavorite()
        {
            //arrange
            var expectedTitle = "Test Article";

            var userTestData = Builder<ZenithUser>
                .CreateListOfSize(1)
                .TheFirst(1)
                .With(u => u.UserName = $"test.user{Faker.RandomNumber.Next()}")
                .Build();


            Context.Users.AddRange(userTestData);
            await Context.SaveChangesAsync();

            var testUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);
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
                .With(a => a.Favorites = new List<Favorite> { new() { UserId = testUser.Id } })
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
            var command = new UnfavoriteArticle.Command(articles[0].Slug);
            var handler = new UnfavoriteArticle.Handler(ServiceMgr,CurrentUserContext,_logger);
            var result = await handler.Handle(command, CancellationToken.None);
            
            //assert
            result.IsSuccess.ShouldBeTrue();
            var favorite = await Context.Favorites
                .FirstOrDefaultAsync(f => f.ArticleId == articles[0].Id && f.UserId == testUser.Id);
            favorite.ShouldBeNull();

        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleExistsAndUserHasNotFavored_ReturnsSuccess()
        {
            //arrange
            var expectedTitle = "Test Article";

            var userTestData = Builder<ZenithUser>
                .CreateListOfSize(1)
                .TheFirst(1)
                .With(u => u.UserName = $"test.user{Faker.RandomNumber.Next()}")
                .Build();


            Context.Users.AddRange(userTestData);
            await Context.SaveChangesAsync();

            var testUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);
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
                .With(a => a.Favorites = new List<Favorite> { new() { UserId = userTestData[0].Id } })
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
            var command = new UnfavoriteArticle.Command(articles[0].Slug);
            var handler = new UnfavoriteArticle.Handler(ServiceMgr,CurrentUserContext,_logger);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeTrue();
            var favorite = await Context.Favorites
                .FirstOrDefaultAsync(f => f.ArticleId == articles[0].Id && f.UserId == testUser.Id);
            favorite.ShouldBeNull();
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleDoesNotExist_ReturnErrorResult()
        {
            //arrange
            var command = new UnfavoriteArticle.Command("test-article");

            //act
            var handler = new UnfavoriteArticle.Handler(ServiceMgr,CurrentUserContext,_logger);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeFalse();
        }
    }
}
