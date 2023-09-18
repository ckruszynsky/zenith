namespace Zenith.Domain.Entities
{
    using System.Collections.Generic;
    using Zenith.SharedKernel;

    public class Tag : EntityBase
    {
        public Tag()
        {
            ArticleTags = new List<ArticleTag>();
        }

        public string Description { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; }
    }
}
