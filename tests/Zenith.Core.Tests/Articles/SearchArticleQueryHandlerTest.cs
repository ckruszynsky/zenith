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
        public async Task GivenValidRequest_WithTag_ReturnsListArticleFeedItemsFilteredByTag()
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
        public async Task GivenValidRequest_WithAuthor_ReturnsListArticleFeedItemsFilteredByAuthor()
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
        public async Task GivenValidRequest_WithAuthorAndTag_ReturnsListArticleFeedItemsFilteredByTagAndAuthor()
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
        public async Task GivenValidRequest_WithSearchText_ReturnsListArticleFeedItemsFilteredBySearchText()
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

        [Fact]
        public async Task GivenValidRequest_WithSearchTextAndTag_ReturnsListArticleFeedItemsFilteredBySearchTextAndTag()
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
                .TheNext(1)
                .With(a => a.Title = "Development Practices")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(8)
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
            var expectedTag = "unit testing";
            var expectedAuthor = userTestData[0].UserName;

            var query = new SearchArticles.Query(
                Tag: expectedTag,
                SearchText: searchText);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ToLowerInvariant().Contains(searchText.ToLowerInvariant()).ShouldBe(true);
            response.Value.First().Tags.Contains(expectedTag).ShouldBeTrue();
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithSearchTextAndTagAndAuthor_ReturnsListArticleFeedItemsFilteredByTagAndAuthorAndSearchText()
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
                .TheFirst(5)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(1)
                .With(a => a.Title = "Modern Software Development")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 1;
            var tagFilter = "unit testing";
            var authorFilter = userTestData[0].UserName;
            var searchText = "Modern";

            var query = new SearchArticles.Query(
                Tag: tagFilter,
                Author: authorFilter,
                SearchText: searchText);

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
        public async Task GivenValidRequest_WithIncludeFavorite_ReturnsListArticleFeedItemsFilteredByFavoritesOnly()
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

            var user = Context.Users.Single(u => u.UserName == TestConstants.TestUserName);
            
            user.Favorites.Add(new Favorite
            {
                ArticleId = articleTestData[0].Id
            });

            await Context.SaveChangesAsync();
                

            var expectedTotalCount = 1;
            var expectedAuthor = userTestData[0].UserName;

            var query = new SearchArticles.Query(
                IncludeOnlyFavorite:true);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ShouldBe(articleTestData[0].Title);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithIncludeFavoriteAndSearchText_ReturnsListArticleFeedItemsFilteredByFavoritesOnlyAndSearchText()
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
                .TheNext(1)
                .With(a => a.Title = "Vertical Slice Design")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(8)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var user = Context.Users.Single(u => u.UserName == TestConstants.TestUserName);

            user.Favorites.Add(new Favorite
            {
                ArticleId = articleTestData[0].Id
            });

            await Context.SaveChangesAsync();


            var expectedTotalCount = 1;
            var expectedAuthor = userTestData[0].UserName;

            var query = new SearchArticles.Query(
                SearchText:"Vertical",
                IncludeOnlyFavorite: true);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ShouldBe(articleTestData[0].Title);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithIncludeFavoriteAndTag_ReturnsListArticleFeedItemsFilteredByFavoritesOnlyAndTag()
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
                .TheNext(1)
                .With(a => a.Title = "Vertical Slice Design")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(8)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var user = Context.Users.Single(u => u.UserName == TestConstants.TestUserName);

            user.Favorites.Add(new Favorite
            {
                ArticleId = articleTestData[0].Id
            });

            await Context.SaveChangesAsync();


            var expectedTotalCount = 1;
            var expectedAuthor = userTestData[0].UserName;
            var expectedTag = "unit testing";
            var query = new SearchArticles.Query(
                Tag:expectedTag,
                IncludeOnlyFavorite: true);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ShouldBe(articleTestData[0].Title);
            response.Value.First().Tags.Contains(expectedTag).ShouldBeTrue();
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenValidRequest_WithIncludeFavoriteAndTagAndAuthor_ReturnsListArticleFeedItemsFilteredByFavoritesOnlyAndTagAndAuthor()
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
                .TheNext(1)
                .With(a => a.Title = "Vertical Slice Design")
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[0].Id } })
                .TheNext(8)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var user = Context.Users.Single(u => u.UserName == TestConstants.TestUserName);

            user.Favorites.Add(new Favorite
            {
                ArticleId = articleTestData[0].Id
            });

            await Context.SaveChangesAsync();


            var expectedTotalCount = 1;
            var expectedAuthor = userTestData[0].UserName;
            var expectedTag = "unit testing";
            var query = new SearchArticles.Query(
                Author:expectedAuthor,
                Tag: expectedTag,
                IncludeOnlyFavorite: true);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.First().Author.ShouldBe(expectedAuthor);
            response.Value.First().Title.ShouldBe(articleTestData[0].Title);
            response.Value.First().Tags.Contains(expectedTag).ShouldBeTrue();
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenRequestForSearchTextOfTitleThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                SearchText:"NonExistent");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenRequestForTagThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();


            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                Tag: "NonExistent");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenRequestForAuthorThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                Author: "NonExistent");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenRequestForFavoriteOnlyThatUserHasNoFavorites_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                IncludeOnlyFavorite:true);

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

        [Fact]
        public async Task GivenRequestForSearchTextAndTagThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                SearchText:"NonExistent",
                Tag:"unit testing");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }


        [Fact]
        public async Task GivenRequestForSearchTextAndTagAndAuthorThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                Author: articleTestData[0].Author.UserName,
                SearchText: "NonExistent",
                Tag: "unit testing");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }


        [Fact]
        public async Task GivenRequestForSearchTextAndTagAndAuthorAndFavoriteThatDoesntExist_ReturnsNoResults()
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
                .TheFirst(10)
                .With(a => a.Title = string.Join(" ", Faker.Lorem.Words(5)))
                .With(a => a.Description = string.Join(" ", Faker.Lorem.Words(25)))
                .With(a => a.Slug = string.Join(" ", Faker.Lorem.Words(5)).ToSlug())
                .With(a => a.AuthorId = userTestData[0].Id)
                .With(a => a.ArticleTags = new List<ArticleTag> { new() { TagId = tagsTestData[1].Id } })
                .Build();

            Context.Articles.AddRange(articleTestData);
            await Context.SaveChangesAsync();

            var expectedTotalCount = 0;
            var query = new SearchArticles.Query(
                Author: articleTestData[0].Author.UserName,
                IncludeOnlyFavorite:true,
                SearchText: "NonExistent",
                Tag: "unit testing");

            var handler = new SearchArticles.Handler(ServiceMgr, Mapper, _logger, CurrentUserContext);

            var response = await handler.Handle(query, CancellationToken.None);

            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.TotalRecords.ShouldBe(expectedTotalCount);
            response.Value.ShouldBeOfType<List<ArticleFeedViewModel>>();
        }

    }
}
