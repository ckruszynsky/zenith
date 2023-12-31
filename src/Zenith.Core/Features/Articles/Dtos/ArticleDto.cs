﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Zenith.Common.Extensions;
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
        
        public bool Favorited { get; set; }

        public bool Following { get; set; }

        public int FavoritesCount { get; set; }

        public int CommentsCount { get; set; }

        public ArticleAuthorDto Author { get; set; } 
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {

            profile.CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(source => source.ArticleTags))
                .ForMember(dest => dest.Author, opts => opts.MapFrom(source => source.Author))
                .ForMember(dest => dest.Favorited, opts => opts.Ignore())
                .ForMember(dest => dest.Following, opts => opts.Ignore());

            profile.CreateMap<ArticleDto,Article>()
                .ForMember(dest => dest.ArticleTags, opts => opts.Ignore())
                .ForMember(dest => dest.AuthorId, opts => opts.Ignore())
                .ForMember(dest => dest.Author, opts => opts.Ignore())
                .ForMember(dest => dest.Created, opts => opts.Ignore())
                .ForMember(dest => dest.LastModified, opts => opts.Ignore())
                .ForMember(dest => dest.Slug, opts=> opts.MapFrom(source => source.Title.ToSlug()));



        }
    }
}
