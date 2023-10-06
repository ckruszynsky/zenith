using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Profile.Contracts;
using Zenith.Core.Features.Profile.Dtos;
using Zenith.Core.Infrastructure.Persistence;
using Ardalis.GuardClauses;
using Zenith.Core.Infrastructure.Identity;
using System.Linq.Expressions;

namespace Zenith.Core.Features.Profile
{
    public class ProfileService:IProfileService
    {
        private readonly AppDbContext _context;        
        private readonly ICurrentUserContext _currentUserContext;

        public ProfileService(AppDbContext context,ICurrentUserContext currentUserContext)
        {
            _context = context;            
            _currentUserContext = currentUserContext;
        }
        public async Task<bool> FollowUser(string username)
        {
            Guard.Against.NullOrEmpty(username, nameof(username));            
            var userToFollow = await GetUser(username);

            var currentUser = await _currentUserContext.GetCurrentUserContext();
            if(currentUser.Id == userToFollow.Id)
            {
                throw new InvalidOperationException("A user cannot follow themselves");
            }

            var existingFollow = userToFollow.Followers
                .FirstOrDefault(userToFollow => userToFollow.UserFollower == currentUser);

            if (existingFollow != null) return true;

            var newFollow = new UserFollow
            {
                UserFollowing = userToFollow,
                UserFollower = currentUser
            };

            userToFollow.Followers.Add(newFollow);
            await _context.SaveChangesAsync();
            return true;
        }
   

        public async Task<bool> UnfollowUser(string username)
        {
            Guard.Against.NullOrEmpty(username, nameof(username));
            
            var userToUnfollow = await GetUser(username);

            var currentUser = await _currentUserContext.GetCurrentUserContext();
            if (currentUser.Id == userToUnfollow.Id)
            {
                throw new InvalidOperationException("A user cannot unfollow themselves");
            }

            var existingFollow = userToUnfollow.Followers
                .FirstOrDefault(userToFollow => userToFollow.UserFollower == currentUser);
            
            if (existingFollow == null) return true;

            userToUnfollow.Followers.Remove(existingFollow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProfileDto> GetProfile(string username)
        {
            var existingUser = await _context.Users
                .Include(x => x.Followers)
                .Where(u=> string.Equals(u.UserName,username,StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new NotFoundException(nameof(ZenithUser), username);
            }

            var profile = new ProfileDto
            {
                UserName = existingUser.UserName,
                Bio = existingUser.Bio,
                Image = existingUser.Image,
                Following = existingUser.Followers.Any(x => x.UserFollower.UserName == username)
            };

            return profile;
        }

        private async Task<ZenithUser?> GetUser(string username)
        {
            var userToFollow = await _context.Users
                .Include(x => x.Followers)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (userToFollow == null)
            {
                throw new NotFoundException(nameof(ZenithUser), username);
            }

            return userToFollow;
        }
    }
}
