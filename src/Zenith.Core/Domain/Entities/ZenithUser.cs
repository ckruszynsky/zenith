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

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }

        public string Image { get; set; }

        public ICollection<UserFollow> Followers { get; }

        public ICollection<UserFollow> Following { get; }

        public ICollection<Favorite> Favorites { get; }

        public ICollection<Article> Articles { get; }
    }
}
