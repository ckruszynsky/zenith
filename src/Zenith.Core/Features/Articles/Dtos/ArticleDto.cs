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
    
    public class ArticleDto:IMapFrom<Article>
    {
        public int Id { get; set; }
        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public List<ArticleTagDto> Tags { get; set; }

        public int FavoritesCount { get; set; }

        public int CommentsCount { get; set; }

        public ArticleAuthorDto Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Article,ArticleDto>()
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(source => source.ArticleTags))
                .ForMember(dest => dest.Author, opts => opts.MapFrom(source => source.Author));
        }
    }
}
