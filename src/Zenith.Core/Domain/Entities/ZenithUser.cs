using Microsoft.AspNetCore.Identity;

namespace Zenith.Core.Domain.Entities
{
    public class ZenithUser : IdentityUser
    {
        public ZenithUser()
        {
            Followers = new List<UserFollow>();
            Following = new List<UserFollow>();
            Favorites = new List<Favorite>();
            Articles = new List<Article>();
        }

        public string? Bio { get; set; } = null;

        public string? Image { get; set; }

        public virtual ICollection<UserFollow> Followers { get; }

        public virtual ICollection<UserFollow> Following { get; }

        public virtual ICollection<Favorite> Favorites { get; }

        public virtual ICollection<Article> Articles { get; }

        public virtual ICollection<Comment> Comments { get; }
       
    }
}
