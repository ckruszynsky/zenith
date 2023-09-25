namespace Zenith.Domain.Entities;
using System.Collections.Generic;
using Zenith.SharedKernel;

public class Article : EntityBase
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public string AuthorId { get; set; }
    public virtual ZenithUser Author { get; set; }
    public ICollection<ArticleTag> ArticleTags { get; set; }
    public ICollection<Favorite> Favorites { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public int FavoritesCount => Favorites.Count;
    public int CommentsCount => Comments.Count;

    public Article()
    {
        ArticleTags = new List<ArticleTag>();
        Favorites = new List<Favorite>();
        Comments = new List<Comment>();
    }

}