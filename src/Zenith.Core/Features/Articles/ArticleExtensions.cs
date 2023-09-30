using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;

namespace Zenith.Core.Features.Articles
{
    public static class ArticleExtensions
    {

        public static bool IsArticleFavoritedByUser(this Article articleEntity, ZenithUser user)
        {
            Guard.Against.Null(articleEntity, nameof(articleEntity));
            return articleEntity.Favorites.Any(f => f.User == user);
        }

        public static bool IsAuthorFollowedByUser(this Article articleEntity, ZenithUser user)
        {
            Guard.Against.Null(articleEntity, nameof(articleEntity));
            return articleEntity.Author.Followers.Any(f => f.UserFollower == user);

        }

        public static void UpdateArticleFollowing(this IEnumerable<ArticleDto> articleDtos, ZenithUser? user, List<Article> articleEntities)
        {
            var articleDtosList = articleDtos.ToList();
            Guard.Against.NullOrEmpty(articleDtosList, nameof(articleDtos));
            Guard.Against.Null(articleEntities, nameof(articleEntities));

            if (user != null)
            {
                foreach (var articleDto in articleDtosList)
                {
                    var entity = articleEntities.Single(a => a.Id == articleDto.Id);
                    articleDto.Following = entity.IsAuthorFollowedByUser(user);
                    articleDto.Favorited = entity.IsArticleFavoritedByUser(user);

                }
            }
        }

        public static IQueryable<Article> SearchArticlesByTitle(this ArticleSearchDto searchParameters, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            Guard.Against.Null(searchParameters, nameof(searchParameters));
            Guard.Against.NullOrEmpty(searchParameters.SearchText, nameof(searchParameters.SearchText));
            return queryable
                .Where(q => q.Title.ToLowerInvariant().Contains(searchParameters.SearchText.ToLowerInvariant()));
        }

        public static IQueryable<Article> GetArticlesByAuthor(this ArticleSearchDto searchParameters, IIncludableQueryable<Article, ICollection<Comment>> queryable)
        {
            Guard.Against.Null(searchParameters, nameof(searchParameters));
            Guard.Against.NullOrEmpty(searchParameters.Author, nameof(searchParameters.Author));
            return queryable
                .Where(q => q.Author.UserName == searchParameters.Author);
        }

    }
}
