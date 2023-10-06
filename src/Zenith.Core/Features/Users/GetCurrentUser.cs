using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Features.Users
{
    public class GetCurrentUser
    {
        public record Query(): IRequest<Result<UserViewModel>>;

        public class Handler : IRequestHandler<Query, Result<UserViewModel>>
        {
            private readonly ICurrentUserContext _currentUser;

            public Handler(ICurrentUserContext currentUser)
            {
                _currentUser = currentUser;
            }
            public async Task<Result<UserViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _currentUser.GetCurrentUserContext();
                    var userViewModel = new UserViewModel
                    {
                        UserName = user.UserName,
                        Bio = user.Bio,
                        Image = user.Image
                    };
                    return Result<UserViewModel>.Success(userViewModel);
                }
                catch(Exception e)
                {
                    return Result<UserViewModel>.Error(e.Message);
                }
            }
        }
    }
}
