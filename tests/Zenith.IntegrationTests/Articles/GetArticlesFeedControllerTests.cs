using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Zenith.Common.Responses;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Models;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests.Articles
{
    using static TestingFixture;
    public class GetArticlesFeedControllerTests:BaseTestFixture
    {
        private async Task CreateTestUserAndLogin(HttpClient httpClient)
        {
            var testUser = new ZenithUser
            {
                UserName = "testuser",
                Email = "testUser@gmail.com",
                NormalizedEmail = "testUser@gmail.com".ToUpperInvariant(),
                NormalizedUserName = "testuser".ToUpperInvariant(),
            };

            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);

            var loginRequest = new LoginUserDto
            {
                Email = "testUser@gmail.com",
                Password = "#password1!",
            };

            await ContentHelper.GetRequestWithAuthorization(httpClient, loginRequest);            
        }

        private async Task CreateArticlesForTest()
        {
            var articleUser = new ZenithUser
            {
                UserName = "articleUser",
                Email = "articleUser@gmail.com"
            };

            await AddAsync<ZenithUser>(articleUser);

            var testArticle = new Article
            {
                Title = "Creating Vertical Slice Architecture",
                Description = "How to use vertical slice architecture in your applications",
                Body = "This is the body of the article",
                Slug = "creating-vertical-slice-architecture",
                AuthorId = articleUser.Id,
                Created = DateTime.Now.AddMinutes(5),
                LastModified = DateTime.Now.AddMinutes(30),
            };

            var testArticle2 = new Article
            {
                Title = "Developing Application in ASPNET Core",
                Description = "How to develop applications using ASPNET Core",
                Body = "This is the body of the article",
                Slug = "developing-application-aspnet-core",
                AuthorId = articleUser.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var testArticle3 = new Article
            {
                Title = "Developing Application in ASPNET Core 2",
                Description = "How to develop applications using ASPNET Core2",
                Body = "This is the body of the article",
                Slug = "developing-application-aspnet-core-2",
                AuthorId = articleUser.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var testArticle4 = new Article
            {
                Title = "The Most Used Design Patterns in .NET",
                Description = "The Most used Design Patterns in .NET",
                Body = "This is the body of the article",
                Slug = "most-used-design-patterns-net",
                AuthorId = articleUser.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var testArticle5 = new Article
            {
                Title = "React Design Patterns",
                Description = "React Design Patterns",
                Body = "This is the body of the article",
                Slug = "react-design-patterns",
                AuthorId = articleUser.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            await AddAsync<Article>(testArticle);
            await AddAsync<Article>(testArticle2);
            await AddAsync<Article>(testArticle3);
            await AddAsync<Article>(testArticle4);
            await AddAsync<Article>(testArticle5);

            var testTag1 = new Tag
            {
                Name = "architecture"
            };

            await AddAsync<Tag>(testTag1);            


            var articleTag1 = new ArticleTag
            {
                ArticleId = testArticle.Id,
                TagId = testTag1.Id
            };

            var articleTag2 = new ArticleTag
            {
                ArticleId = testArticle2.Id,
                TagId = testTag1.Id
            };

            var articleTag3 = new ArticleTag
            {
                ArticleId = testArticle3.Id,
                TagId = testTag1.Id
            };

            var articleTag4 = new ArticleTag
            {
                ArticleId = testArticle4.Id,
                TagId = testTag1.Id
            };

            var articleTag5 = new ArticleTag
            {
                ArticleId = testArticle5.Id,
                TagId = testTag1.Id
            };

            await AddAsync<ArticleTag>(articleTag1);
            await AddAsync<ArticleTag>(articleTag2);
            await AddAsync<ArticleTag>(articleTag3);
            await AddAsync<ArticleTag>(articleTag4);
            await AddAsync<ArticleTag>(articleTag5);

        }

        [Test]
        public async Task GivenValidRequest_WhenGetArticlesFeed_ThenReturnSuccess()
        {
            //arrange
            var client = GetHttpClient();
            await CreateTestUserAndLogin(client);
            await CreateArticlesForTest();


            //act
            var response = await client.GetAsync(ArticlesEndpoint);
            var responseContent = await ContentHelper.GetResponseContent<PaginatedList<ArticleFeedViewModel>>(response);

            //assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<PaginatedList<ArticleFeedViewModel>>();
            responseContent.Items.Count.ShouldBe(5);
        }

        [Test]
        public async Task GivenValidRequest_WhenRequestContainsPageSize_ThenReturnsCorrectNumberOfArticles()
        {
            //arrange
            var client = GetHttpClient();
            await CreateTestUserAndLogin(client);
            await CreateArticlesForTest();

            //act
            var response = await client.GetAsync($"{ArticlesEndpoint}?pageSize=1");
            var responseContent = await ContentHelper.GetResponseContent<PaginatedList<ArticleFeedViewModel>>(response);

            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<PaginatedList<ArticleFeedViewModel>>();
            responseContent.PageNumber.ShouldBe(1);
            responseContent.Items.Count.ShouldBe(1);
        }

        [Test]
        public async Task GivenValidRequest_WhenRequestContainsPageNumber_ThenReturnsPageOfResults()
        {
            //arrange
            var client = GetHttpClient();
            await CreateTestUserAndLogin(client);
            await CreateArticlesForTest();

            //act
            var response = await client.GetAsync($"{ArticlesEndpoint}?pageSize=1&pageNumber=2");
            var responseContent = await ContentHelper.GetResponseContent<PaginatedList<ArticleFeedViewModel>>(response);

            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<PaginatedList<ArticleFeedViewModel>>();
            responseContent.PageNumber.ShouldBe(2);
            responseContent.Items.Count.ShouldBe(1);
        }

        [Test]
        public async Task GivenValidRequest_WhenRequestContainsTagId_ThenReturnsResultsForTag()
        {
            //arrange
            var client = GetHttpClient();
            await CreateTestUserAndLogin(client);
            await CreateArticlesForTest();

            var articleUser = new ZenithUser
            {
                UserName = "articleUser2",
                Email = "articleUser2@gmail.com"
            };

            await AddAsync<ZenithUser>(articleUser);

            var testArticle = new Article
            {
                Title = "Test Article",
                Description = "Test Article",
                Body = "This is the body of the article",
                Slug = "test-article",
                AuthorId = articleUser.Id,
                Created = DateTime.Now.AddMinutes(5),
                LastModified = DateTime.Now.AddMinutes(30),
            };

            await AddAsync<Article>(testArticle);

            var testTag = new Tag
            {
                Name = "test-tag"
            };

            await AddAsync<Tag>(testTag);

            var articleTag = new ArticleTag
            {
                ArticleId = testArticle.Id,
                TagId = testTag.Id
            };


            await AddAsync<ArticleTag>(articleTag);

            //act
            var response = await client.GetAsync($"{ArticlesEndpoint}?tagId={testTag.Id}");
            var responseContent = await ContentHelper.GetResponseContent<PaginatedList<ArticleFeedViewModel>>(response);

            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<PaginatedList<ArticleFeedViewModel>>();
            responseContent.Items.Count.ShouldBe(1);
            responseContent.Items.First().Title.ShouldBe(testArticle.Title);
            responseContent.PageNumber.ShouldBe(1);            
        }
    }
}
