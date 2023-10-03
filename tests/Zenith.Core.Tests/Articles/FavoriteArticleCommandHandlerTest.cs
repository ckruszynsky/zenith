using FizzWare.NBuilder;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class FavoriteArticleCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<FavoriteArticle.Handler> _logger;

        public FavoriteArticleCommandHandlerTest()
        {
            _logger = NullLogger<FavoriteArticle.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleExists_ArticleIsFavorited()
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

            var article = await Context.Articles.FirstOrDefaultAsync(a => a.Slug == expectedTitle.ToSlug());
            //act
            var command = new FavoriteArticle.Command(article.Slug);
            var handler = new FavoriteArticle.Handler(ServiceMgr,CurrentUserContext,_logger);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeTrue();
            var favorite = await Context.Favorites
                .Include(f=> f.Article)
                .Include(f=> f.User)
                .FirstOrDefaultAsync(f => f.ArticleId == article.Id);

            favorite.ShouldNotBeNull();
            favorite.ArticleId.ShouldBe(article.Id);
            favorite.User.UserName.ShouldBe(TestConstants.TestUserName);
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleDoesntExist_ReturnErrorResult()
        {
            //arrange 
            var command = new FavoriteArticle.Command("test-article");

            //act
            var handler = new FavoriteArticle.Handler(ServiceMgr,CurrentUserContext,_logger);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeFalse();
        }
    }
}
