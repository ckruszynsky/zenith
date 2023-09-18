using Zenith.SharedKernel;

namespace Zenith.Domain.Entities
{
    public class ArticleTag : EntityBase
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }


    }
}
