using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Core.Features.Tags.Dtos
{
    public class TagListDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<TagDto> Tags { get; set; }
    }
}
