using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Mapping;
using Zenith.Core.Features.Profile.Dtos;

namespace Zenith.Core.Features.Profile.ViewModel
{
    public class ProfileViewModel:IMapFrom<ProfileDto>
    {
        public string UserName { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public bool Following { get; set; }
    }
}
