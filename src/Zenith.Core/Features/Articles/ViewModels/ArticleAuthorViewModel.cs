using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;

namespace Zenith.Core.Features.Articles.ViewModels
{
    public class ArticleAuthorViewModel: IMapFrom<ZenithUser>
    {
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public bool Following { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<ArticleAuthorDto, ArticleAuthorViewModel>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(source => source.UserName))
                .ForMember(dest => dest.Bio, opts => opts.MapFrom(source => source.Bio))
                .ForMember(dest => dest.Image, opts => opts.MapFrom(source => source.Image));
        }
    }
}
