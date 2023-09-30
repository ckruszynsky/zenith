using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Tags.Dtos
{
    public class TagDto:IMapFrom<Tag>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
    }
}
