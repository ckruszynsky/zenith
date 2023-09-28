using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Shouldly;
using Zenith.Common.Extensions;
using Zenith.Core.Domain.Entities;
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
            //arrange
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
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();
            
            var expectedTotalCount = 6;
            var tagFilter = "unit testing";

            //act
            var query = new SearchArticles.Query(Tag:tagFilter);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            //assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.Any(a=> a.Tags.Contains(tagFilter)).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }


        [Fact]
        public async Task GivenValidRequest_WithAuthorAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredByAuthor()
        {

            //arrange
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
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 6;
            var authorFilter = userTestData[0].UserName;

            //act 
            var query = new SearchArticles.Query(Author:authorFilter);
            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);
            var response = await handler.Handle(query, CancellationToken.None);

            //assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(authorFilter);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithAuthorAndTagAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredByTagAndAuthor()
        {
            //arrange
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
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 6;
            var tagFilter = "unit testing";
            var authorFilter = userTestData[0].UserName;

            var query = new SearchArticles.Query(
                Tag:tagFilter,
                Author:authorFilter);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(authorFilter);
            response.Value.Any(a => a.Tags.Contains(tagFilter)).ShouldBeTrue();
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithSearchTextAndDefaultPageParameters_ReturnsListArticleFeedItemsFilteredBySearchText()
        {
            //arrange
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
                .Build();

            Context.Tags.AddRange(tagsTestData);
            await Context.SaveChangesAsync();

            var articleTestData = Builder<Article>
                .CreateListOfSize(10)
                .TheFirst(1)
                .With(a => a.Title = "Vertical Slice Architecture")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(9)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();
            
            var expectedTotalCount = 1;
            var searchText = "Vertical";
            var expectedAuthor = userTestData[0].UserName;

            var query = new SearchArticles.Query(
                SearchText: searchText);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ToLowerInvariant().Contains(searchText.ToLowerInvariant()).ShouldBe(true);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }
    }
}
