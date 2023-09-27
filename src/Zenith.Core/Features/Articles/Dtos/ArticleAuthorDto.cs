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
    public class ArticleAuthorDto:IMapFrom<ZenithUser>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }

    }
}
