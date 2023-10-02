using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.Tags.Contracts;
using Zenith.Core.Features.Tags.Dtos;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Tags
{
    public class TagService: ITagService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public TagService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TagDto>> CreateTagsAsync(IEnumerable<string> tags)
        {
            var tagDtos = new List<TagDto>();
            if (!tags.Any()) return tagDtos;

            foreach (var tagName in tags)
            {
                var tag = _appDbContext.Tags.FirstOrDefault(x => x.Name == tagName);
                if (tag == null)
                {
                    tag = new Domain.Entities.Tag
                    {
                        Name = tagName
                    };

                    await _appDbContext.Tags.AddAsync(tag);
                    await _appDbContext.SaveChangesAsync();
                    tagDtos.Add(_mapper.Map<TagDto>(tag));
                }
                else
                {
                    tagDtos.Add(_mapper.Map<TagDto>(tag));                    
                }
            }
            return tagDtos;
        }
    }
}
