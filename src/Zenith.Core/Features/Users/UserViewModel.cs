using AutoMapper;
using Zenith.Common.Mapping;
using Zenith.Core.Domain.Entities;

namespace Zenith.Core.Features.Users
{
    public class UserViewModel : IMapFrom<ZenithUser>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<UserViewModel,ZenithUser>()
                .ForMember(u => u.UserName, m => m.MapFrom(u => u.UserName))
                .ForMember(u => u.Email, m => m.MapFrom(u => u.Email))
                .ForMember(u => u.Bio, m => m.MapFrom(u => u.Bio))
                .ForMember(u => u.Image, m => m.MapFrom(u => u.Image));

            profile.CreateMap<ZenithUser, UserViewModel>()
                .ForMember(u => u.UserName, m => m.MapFrom(u => u.UserName))
                .ForMember(u => u.Email, m => m.MapFrom(u => u.Email));
        }
    }
}
