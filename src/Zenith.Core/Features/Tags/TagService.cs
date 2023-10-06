using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zenith.Core.Features.Tags.Contracts;
using Zenith.Core.Features.Tags.Dtos;
using Zenith.Core.Infrastructure.Persistence;
using Ardalis.GuardClauses;

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

        public async Task<TagListDto> GetAllTagsAsync(
            int currentPage, int pageSize)
        {
            Guard.Against.Negative(currentPage, nameof(currentPage));
            Guard.Against.Negative(pageSize, nameof(pageSize));

            var noResults = new TagListDto
            {
                TotalCount = 0,
                Tags = new List<TagDto>()
            };
            
            var tags = await _appDbContext.Tags
                .Skip(currentPage * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tagDtos = _mapper.Map<IEnumerable<TagDto>>(tags);
            
            if(!tagDtos.Any()) return noResults;
             
            return new TagListDto
            {
                Tags = tagDtos,
                TotalCount = await _appDbContext.Tags.CountAsync()
            };            
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
