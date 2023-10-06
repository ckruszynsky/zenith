using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Zenith.Core.Features.Profile.ViewModel;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Profile
{
    public class GetProfile
    {
        public record Query(string Username) : IRequest<Result<ProfileViewModel>>;

        public class Handler : IRequestHandler<Query, Result<ProfileViewModel>>
        {
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IServiceManager _serviceManager;

            public Handler(IMapper mapper, ILogger<Handler> logger, IServiceManager serviceManager)
            {
                _mapper = mapper;
                _logger = logger;
                _serviceManager = serviceManager;
            }
            public async Task<Result<ProfileViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var profileDto = await _serviceManager.Profiles.GetProfile(request.Username);
                    var profileViewModel = _mapper.Map<ProfileViewModel>(profileDto);
                    return Result<ProfileViewModel>.Success(profileViewModel);
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error occurred retrieving profile");
                    return Result.Error("Error occurred retrieving profile");                    
                }
            }
        }
    }
}
