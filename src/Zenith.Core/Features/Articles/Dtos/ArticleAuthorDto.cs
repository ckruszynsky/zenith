using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class ArticleAuthorDto:IMapFrom<ZenithUser>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public bool Following { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ZenithUser, ArticleAuthorDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(source => source.Id))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.UserName))
                .ForMember(dest => dest.Bio, opts => opts.MapFrom(source => source.Bio))
                .ForMember(dest => dest.Image, opts => opts.MapFrom(source => source.Image))
                .ForMember(dest => dest.Following, opts => opts.Ignore());


        }
    }
}
