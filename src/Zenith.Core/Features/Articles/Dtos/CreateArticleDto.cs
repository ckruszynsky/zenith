using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
namespace Zenith.Core.Features.Articles.Dtos
{
    public class CreateArticleDto: IMapFrom<Article>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public IEnumerable<string> TagList { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateArticleDto, Article>()
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.Body, opts => opts.MapFrom(src => src.Body))
                .ForMember(dest => dest.ArticleTags, opts =>opts.Ignore());
                
        }
    }
}
