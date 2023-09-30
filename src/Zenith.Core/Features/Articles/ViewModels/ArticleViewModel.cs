using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Articles.Dtos;

namespace Zenith.Core.Features.Articles.ViewModels
{
    public class ArticleViewModel : IMapFrom<ArticleDto>
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool Favorited { get; set; }
        public bool Following { get; set; }
        public int FavoriteCount { get; set; }
        public ArticleAuthorViewModel Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<ArticleDto, ArticleViewModel>()
                .ForMember(dest => dest.Author, opts => opts.MapFrom(source => source.Author))
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(source => source.Tags.Select(x => x.Name)));
        }
    }
}