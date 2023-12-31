﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Infrastructure.Persistence
{
    public class AppDbInitializer
    {
        private readonly ILogger<AppDbInitializer> _logger;        

        public AppDbInitializer(ILogger<AppDbInitializer> logger)
        {
            _logger = logger;            
        }

        public async Task InitialiseAsync(AppDbContext context)
        {
            try
            {
                if (context.Database.IsRelational())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }


        public static void Initialize(AppDbContext context)
        {
            SeedEntities(context);
        }

        public static void SeedEntities(AppDbContext context)
        {
            SeedUsers(context, out string userId, out string testUserId);
            //SeedUserFollows(context, testUserId, userId);
            //SeedArticles(context, userId, out int articleId);
            //SeedFavorites(context, articleId, userId);
            //SeedTags(context, articleId);
            //SeedComments(context, userId, articleId);

        }

        public static void SeedUsers(AppDbContext context, out string userId, out string testUserId)
        {
            var testUser1 = new ZenithUser
            {
                Email = "test.user@gmail.com",
                NormalizedEmail = "test.user@gmail.com".ToUpperInvariant(),
                UserName = "test.user",
                NormalizedUserName = "test.user".ToUpperInvariant(),
                Bio = "Zenith Test User Account",
                Image = "",
                SecurityStamp = "secretSecurityStamp"
            };
            testUser1.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser1, "#passwordTest1!");

            var testUser2 = new ZenithUser
            {
                Email = "test.user2@gmail.com",
                NormalizedEmail = "test.user2@gmail.com".ToUpperInvariant(),
                UserName = "test.user2",
                NormalizedUserName = "test.user2".ToUpperInvariant(),
                Bio = "Zenith Test User Account",
                Image = "",
                SecurityStamp = "secretSecurityStamp"
            };

            testUser2.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser2, "#passwordTest2!");

            var testUser1Exists = context.Users.Any(usr => string.Equals(usr.UserName, testUser1.UserName, StringComparison.OrdinalIgnoreCase));
            var testUser2Exists = context.Users.Any(u => string.Equals(u.UserName, testUser2.UserName, StringComparison.OrdinalIgnoreCase));
            if (!testUser1Exists)
            {
                context.Users.Add(testUser1);
            }
            if (!testUser2Exists) {             
                context.Users.Add(testUser2);
            }

            userId = testUser1.Id;
            testUserId = testUser2.Id;

            context.SaveChanges();
        }

        private static void SeedArticles(AppDbContext context, string userId, out int articleId)
        {
            var testArticle = new Article
            {
                Title = "Creating Vertical Slice Architecture",
                Description = "How to use vertical slice architecture in your applications",
                Body = "This is the body of the article",
                Slug = "creating-vertical-slice-architecture",
                AuthorId = userId,
                Created = DateTime.Now.AddMinutes(5),
                LastModified = DateTime.Now.AddMinutes(30),
            };

            var testArticle2 = new Article
            {
                Title = "Developing Application in ASPNET Core",
                Description = "How to develop applications using ASPNET Core",
                Body = "This is the body of the article",
                Slug = "developing-application-aspnet-core",
                AuthorId = context.Users.FirstOrDefault(u => string.Equals(u.UserName, "test.user2", StringComparison.OrdinalIgnoreCase))?.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var testArticle3 = new Article
            {
                Title = "Developing Application in ASPNET Core 2",
                Description = "How to develop applications using ASPNET Core2",
                Body = "This is the body of the article",
                Slug = "developing-application-aspnet-core-2",
                AuthorId = context.Users.FirstOrDefault(u => string.Equals(u.UserName, "test.user2", StringComparison.OrdinalIgnoreCase))?.Id,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            context.Articles.AddRange(testArticle, testArticle2, testArticle3);
            context.SaveChanges();
            articleId = testArticle.Id;

        }

        private static void SeedTags(AppDbContext context, int articleId)
        {
            var testTag1 = new Tag
            {
                Name = "architecture"
            };

            var testTag2 = new Tag
            {
                Name = "vertical slice"
            };

            var testTag3 = new Tag
            {
                Name = "development"
            };

            var testTag4 = new Tag
            {
                Name = "clean code"
            };

            context.Tags.Add(testTag1);
            context.Tags.Add(testTag2);
            context.Tags.Add(testTag3);
            context.Tags.Add(testTag4);
            context.SaveChanges();

            var testArticleTag1 = new ArticleTag
            {
                ArticleId = articleId,
                TagId = testTag3.Id
            };

            var testArticleTag2 = new ArticleTag
            {
                ArticleId = articleId,
                TagId = testTag2.Id
            };

            var testArticleTag3 = new ArticleTag
            {
                ArticleId = context.Articles.FirstOrDefault(a => a.Slug == "developing-application-aspnet-core").Id,
                TagId = testTag1.Id
            };

            var testArticleTag4 = new ArticleTag
            {
                ArticleId = context.Articles.FirstOrDefault(a => a.Slug == "developing-application-aspnet-core-2").Id,
                TagId = testTag1.Id
            };

            
            context.ArticleTags.Add(testArticleTag1);
            context.ArticleTags.Add(testArticleTag2);
            context.ArticleTags.Add(testArticleTag3);
            context.ArticleTags.Add(testArticleTag4);
            context.SaveChanges();
        }

        private static void SeedComments(AppDbContext context, string userId, int articleId)
        {
            var comment = new Comment
            {
                Body = "Thank you so much!",
                UserId = userId,
                ArticleId = articleId,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var comment2 = new Comment
            {
                Body = "This article is terrible!",
                UserId = context.Users.FirstOrDefault(u => u.UserName == "test.user2")?.Id,
                ArticleId = articleId,
                Created = DateTime.Now.AddMinutes(5),
                LastModified = DateTime.Now.AddMinutes(5)
            };

            context.Comments.AddRange(comment, comment2);
            context.SaveChanges();
        }

        private static void SeedUserFollows(AppDbContext context, string followerId, string followeeId)
        {
            var userFollower = context.Users.Find(followerId);
            var userFollowee = context.Users.Find(followeeId);
            var userFollow = new UserFollow
            {
                UserFollower = userFollower,
                UserFollowing = userFollowee
            };

            userFollower.Followers.Add(userFollow);
            context.UserFollows.Add(userFollow);
            context.SaveChanges();
        }

        private static void SeedFavorites(AppDbContext context, int articleId, string userId)
        {
            var favorite = new Favorite
            {
                ArticleId = articleId,
                UserId = userId
            };
            context.Favorites.Add(favorite);

            // Update the favorite count for the article and save the changes
            var article = context.Articles.Find(articleId);
            context.Articles.Update(article);

            context.SaveChanges();
        }
    }
}