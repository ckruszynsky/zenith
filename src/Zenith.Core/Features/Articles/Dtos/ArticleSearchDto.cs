using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class ArticleSearchDto
    {
        public string? Tag { get; set; } = null;
        public string? Author { get; set; } = null;
        public bool? IncludeOnlyFavorites { get; set; } = false;
        public string? SearchText { get; set; } = null;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
 