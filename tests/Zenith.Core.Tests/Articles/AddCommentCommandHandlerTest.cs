using FizzWare.NBuilder;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class AddCommentCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<AddComment.Handler> _logger;

        public AddCommentCommandHandlerTest()
        {
            _logger = NullLogger<AddComment.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheArticleExists_AddCommentToArticle()
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

            //act
            var command = new AddComment.Command(articles[0].Slug, new AddCommentDto { Body = "Test Comment" });
            var handler = new AddComment.Handler(ServiceMgr,CurrentUserContext,_logger,Mediator);

            //assert
            var result = await handler.Handle(command, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheArticleDoesNotExist_ReturnsErrorResult()
        {
            //arrange
            var newComment = new AddCommentDto { Body = "Test Comment" };
            var command = new AddComment.Command("test-article", newComment);
            var handler = new AddComment.Handler(ServiceMgr, CurrentUserContext, _logger, Mediator);
            //act
            var result = await handler.Handle(command, CancellationToken.None);
            //assert
            result.IsSuccess.ShouldBeFalse();
            result.Errors.Contains($"Article with slug {"test-article".ToSlug()} not found").ShouldBeTrue();

        }
    }
}
