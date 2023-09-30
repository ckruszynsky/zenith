using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.Tags.Dtos;

namespace Zenith.Core.Features.Tags.Contracts
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> CreateTagsAsync(IEnumerable<string> tags);
    }
}
    