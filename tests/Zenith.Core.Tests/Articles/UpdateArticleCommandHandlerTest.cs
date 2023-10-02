using FizzWare.NBuilder;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Articles
{
    public class UpdateArticleCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<UpdateArticle.Handler> _logger;

        public UpdateArticleCommandHandlerTest()
        {
            _logger = NullLogger<UpdateArticle.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleSlugExists_ReturnsSuccessAndUpdatesArticle()
        {
            // Arrange
            var initialTitle = "Test Article";

            var author = Context.Users.FirstOrDefault(u => u.UserName == TestConstants.TestUserName);


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
                .With(a => a.Title = initialTitle)
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = initialTitle.ToSlug())
                .With(a => a.AuthorId = author.Id )
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheRest()
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = author.Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articles);
            await Context.SaveChangesAsync();

            var updatedArticle = new UpdateArticleDto
            {
                Body = "This is the updated body",
                Description = "This is the updated description",
                Title = "This is the updated title",
                Tags = new List<string> { "unit testing", "dot net development-2" }
            };

            var command = new UpdateArticle.Command(initialTitle.ToSlug(), updatedArticle);
            var handler = new UpdateArticle.Handler(ServiceMgr,Mapper,CurrentUserContext,_logger);

            //act
            var response = await handler.Handle(command, CancellationToken.None);

            //assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.Title.ShouldBe(updatedArticle.Title);
            response.Value.Description.ShouldBe(updatedArticle.Description);
            response.Value.Body.ShouldBe(updatedArticle.Body);
            response.Value.Slug.ShouldBe(updatedArticle.Title.ToSlug());
            response.Value.Tags.Count().ShouldBe(2);
            response.Value.Tags.Contains("unit testing").ShouldBeTrue();
            response.Value.Tags.Contains("dot net development-2").ShouldBeTrue();
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleSlugDoesNotExist_ReturnsError()
        {
            // Arrange
            var initialTitle = "Test Article";
            var updatedArticle = new UpdateArticleDto
            {
                Body = "This is the updated body",
                Description = "This is the updated description",
                Title = "This is the updated title",
            };
            
            var command = new UpdateArticle.Command(initialTitle.ToSlug(), updatedArticle);
            var handler = new UpdateArticle.Handler(ServiceMgr, Mapper, CurrentUserContext, _logger);

            //act
            var response = await handler.Handle(command, CancellationToken.None);

            //assert
            response.IsSuccess.ShouldBeFalse();
            response.Value.ShouldBeNull();
            response.Errors.Contains($"Article with slug {initialTitle.ToSlug()} not found").ShouldBeTrue();
        }

        [Fact]
        public async Task GivenValidRequest_WhenArticleExistsAndUserIsNotAuthor_ReturnsErrorResponse()
        {
            // Arrange
            var initialTitle = "Test Article";
            var updatedArticle = new UpdateArticleDto
            {
                Body = "This is the updated body",
                Description = "This is the updated description",
                Title = "This is the updated title",
            };


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
                .With(a => a.Title = initialTitle)
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = initialTitle.ToSlug())
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
            var command = new UpdateArticle.Command(initialTitle.ToSlug(), updatedArticle);
            var handler = new UpdateArticle.Handler(ServiceMgr, Mapper, CurrentUserContext, _logger);
            
            var response = await handler.Handle(command, CancellationToken.None);

            //assert
            response.IsSuccess.ShouldBeFalse();
            response.Value.ShouldBeNull();
            response.Errors.Contains($"Article with slug {initialTitle.ToSlug()} not found").ShouldBeTrue();
        }
    }
}
