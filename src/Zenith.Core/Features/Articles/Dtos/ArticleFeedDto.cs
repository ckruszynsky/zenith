using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class ArticleFeedDto
    {
        public int? TagId { get; set; } = null;
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
