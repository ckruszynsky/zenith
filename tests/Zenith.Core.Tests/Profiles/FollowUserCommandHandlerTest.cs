using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Zenith.Core.Features.Profile;
using Zenith.Core.Tests.Infrastructure;
using FizzWare.NBuilder;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Zenith.Core.Tests.Profiles
{
    public class FollowUserCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<FollowUser.Handler> _logger;

        public FollowUserCommandHandlerTest()
        {
            _logger = NullLogger<FollowUser.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheFollowerExists_ReturnsSuccess()
        {
            //arrange
            var testUser = Builder<ZenithUser>.CreateNew()
                .With(u=> u.UserName = "testFollowedUser")
                .Build();

            Context.Users.Add(testUser);
            await Context.SaveChangesAsync();

            var currentUser = CurrentUserContext.GetCurrentUserContext();

            //act
            var command = new FollowUser.Command(testUser.UserName);
            var handler = new FollowUser.Handler(Mapper,ServiceMgr,_logger,Mediator);
            var result = await handler.Handle(command, CancellationToken.None);
            
            //assert
            result.IsSuccess.ShouldBeTrue();

            var user = Context.Users
                .Include(u=> u.Followers)
                .FirstOrDefault(u => u.UserName == testUser.UserName);

            user.Followers.Count.ShouldBe(1);
            user.Followers.First().UserFollower.UserName.ShouldBe(currentUser.UserName);

        }

        [Fact]
        public async Task GivenValidRequest_WhenFollowerDoesNotExist_ReturnsError()
        {
            //arrange

            var currentUser = CurrentUserContext.GetCurrentUserContext();

            //act
            var command = new FollowUser.Command("missingUserName");
            var handler = new FollowUser.Handler(Mapper, ServiceMgr, _logger, Mediator);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeFalse();
            var userFollows = Context.UserFollows.ToList();            
            userFollows.Count.ShouldBe(0);            
        }

        [Fact]
        public async Task GivenValidRequest_WhenFollowerIsSameAsUser_ReturnsError()
        {
            //arrange

            var currentUser = CurrentUserContext.GetCurrentUserContext();

            //act
            var command = new FollowUser.Command(currentUser.UserName);
            var handler = new FollowUser.Handler(Mapper, ServiceMgr, _logger, Mediator);
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeFalse();
            var userFollows = Context.UserFollows.ToList();
            userFollows.Count.ShouldBe(0);
        }
    }
}
