using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Zenith.Common.Mapping;
using Zenith.Core.Features.Articles.Dtos;

namespace Zenith.Core.Features.Articles.Models
{
    public class ArticleFeedViewModel: IMapFrom<ArticleDto>
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public bool Favorited { get; set; }
        public bool Following { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<ArticleDto,ArticleFeedViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(source => source.Author.UserName))
                .ForMember(dest => dest.AuthorImage, opts => opts.MapFrom(source => source.Author.Image))
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(source => source.Tags.Select(x=> x.Name)));
        }
    }
}
