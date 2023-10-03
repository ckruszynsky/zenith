using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class DeleteCommentCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<DeleteComment.Handler> _logger;

        public DeleteCommentCommandHandlerTest()
        {
            _logger = NullLogger<DeleteComment.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheCommentExistAndIsOwner_CommentIsDeleted()
        {
            //arrange
            var expectedTitle = "Test Article";

            var userTestData = Builder<ZenithUser>
                .CreateListOfSize(1)
                .TheFirst(1)
                .With(u => u.UserName = $"test.user{Faker.RandomNumber.Next()}")
                .Build();

            var testUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);

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
                .With(a => a.Comments = new List<Comment> { new() { Body = "Test Comment", UserId = testUser.Id } })
                .TheRest()
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })                
                .Build();

            Context.Articles.AddRange(articles);
            await Context.SaveChangesAsync();
            
            var comment = articles[0].Comments.FirstOrDefault();
            var command = new DeleteComment.Command(articles[0].Slug, comment.Id);
            var handler = new DeleteComment.Handler(ServiceMgr,CurrentUserContext,_logger);

            //act
            var response = handler.Handle(command, default);

            //assert
            Context.Comments.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheCommentDoesNotExist_ReturnsErrorResult()
        {
            //arrange
            var command = new DeleteComment.Command("test-article", 1);
            var handler = new DeleteComment.Handler(ServiceMgr, CurrentUserContext, _logger);
            
            //act
            var response = await handler.Handle(command, default);

            //assert
            response.IsSuccess.ShouldBeFalse();
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheCommentIsNotOwnedByTheRequester_ReturnsErrorResult()
        {
            var expectedTitle = "Test Article";

            var userTestData = Builder<ZenithUser>
                .CreateListOfSize(1)
                .TheFirst(1)
                .With(u => u.UserName = $"test.user{Faker.RandomNumber.Next()}")
                .Build();

            var testUser = await Context.Users.FirstOrDefaultAsync(u => u.UserName == TestConstants.TestUserName);

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
                .With(a => a.Comments = new List<Comment> { new() { Body = "Test Comment", UserId = userTestData[0].Id } })
                .TheRest()
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articles);
            await Context.SaveChangesAsync();

            var comment = articles[0].Comments.FirstOrDefault();
            var command = new DeleteComment.Command(articles[0].Slug, comment.Id);
            var handler = new DeleteComment.Handler(ServiceMgr, CurrentUserContext, _logger);

            //act   
            var response = await handler.Handle(command, default);

            //assert
            response.IsSuccess.ShouldBeFalse();
        }

    }
}
