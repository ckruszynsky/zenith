using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users;

namespace Zenith.Core.Features.Profile.Dtos
{
    public class ProfileDto: IMapFrom<ZenithUser>
    {
        public string UserName { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public bool Following { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
                profile.CreateMap<ZenithUser, ProfileDto>()
                    .ForMember(u => u.UserName, m => m.MapFrom(u => u.UserName))
                    .ForMember(u => u.Bio, m => m.MapFrom(u => u.Bio))
                    .ForMember(u => u.Image, m=> m.MapFrom(u => u.Image))
                    .ForMember(u => u.Following, m=> m.Ignore());
            
        }
    }
}
