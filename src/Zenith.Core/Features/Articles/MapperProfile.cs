using AutoMapper;
using Zenith.Domain.Entities;

namespace Zenith.Core.Features.Articles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Article, GetArticlesFeed.ArticleFeedItem>()
                .ForMember(a => a.Author, opts => opts.MapFrom(a => a.Author.NormalizedUserName));
        }
    }
}
