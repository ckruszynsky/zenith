using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class ArticleListDto
    {
        public IEnumerable<ArticleDto> Articles { get; set; }
        public int TotalCount { get; set; }

    }
}
