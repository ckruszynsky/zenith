using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Articles.Dtos
{
    public class AddCommentDto:IMapFrom<Comment>
    {
        public string Body { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<AddCommentDto, Comment>()
                .ForMember(d => d.Body, opt => opt.MapFrom(s => s.Body))
                .ForMember(d => d.ArticleId, opt => opt.Ignore())
                .ForMember(d => d.Article, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModified, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());
                
        }
    }
}
