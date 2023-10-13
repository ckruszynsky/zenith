using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.Features.Users
{
    public class GetCurrentUser
    {
        public record Query(): IRequest<Result<UserViewModel>>;

        public class Handler : IRequestHandler<Query, Result<UserViewModel>>
        {
            private readonly ICurrentUserContext _currentUser;
            private readonly AppDbContext _appDbContext;

            public Handler(ICurrentUserContext currentUser, AppDbContext appDbContext)
            {
                _currentUser = currentUser;
                _appDbContext = appDbContext;
            }
            public async Task<Result<UserViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var claimsPrincipal = _currentUser.GetCurrentUserContext();
                    var user = await _appDbContext.Users.FirstAsync(u => u.UserName ==claimsPrincipal.UserName);
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
