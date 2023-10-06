using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class ArticleTagDto: IMapFrom<ArticleTag>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<ArticleTag, ArticleTagDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Tag.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Tag.Name));
        }
    }
}
