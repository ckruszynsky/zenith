using Zenith.Common.Domain;

namespace Zenith.Core.Domain.Entities;

public class Article : BaseAuditableEntity
{
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Body { get; set; }
    public required string AuthorId { get; set; }
    public virtual ZenithUser Author { get; set; }
    public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public int FavoritesCount => Favorites.Count;
    public int CommentsCount => Comments.Count;
}