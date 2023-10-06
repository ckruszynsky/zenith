using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Tags.Dtos;

namespace Zenith.Core.Features.Tags.ViewModels
{
    public class TagViewModel:IMapFrom<TagDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
