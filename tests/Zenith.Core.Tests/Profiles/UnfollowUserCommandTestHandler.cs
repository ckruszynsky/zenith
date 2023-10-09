using AutoMapper;
using FizzWare.NBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Profile;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Profiles;
public class UnfollowUserCommandHandlerTest : TestFixture
{
    private readonly NullLogger<UnfollowUser.Handler> _logger;

    public UnfollowUserCommandHandlerTest()
    {
        _logger = NullLogger<UnfollowUser.Handler>.Instance;
    }

    [Fact]
    public async Task GivenValidRequest_WhenTheFollowerExists_ReturnsSuccess()
    {
        //arrange
        var testUser = Builder<ZenithUser>.CreateNew()
            .With(u => u.UserName = "testFollowedUser")
            .Build();

        Context.Users.Add(testUser);
        await Context.SaveChangesAsync();

        

        var currentUser = await CurrentUserContext.GetCurrentUserContext();

        testUser.Followers.Add(new UserFollow
        {
            UserFollower = currentUser
        });

        await Context.SaveChangesAsync();


        //act
        var command = new UnfollowUser.Command(testUser.UserName);
        var handler = new UnfollowUser.Handler(Mapper, ServiceMgr, _logger, Mediator);
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.ShouldBeTrue();

        var user = Context.Users
            .Include(u => u.Followers)
            .FirstOrDefault(u => u.UserName == testUser.UserName);

        user.Followers.Count.ShouldBe(0);
        

    }

    [Fact]
    public async Task GivenValidRequest_WhenFollowerDoesNotExist_ReturnsError()
    {
        //arrange

        var currentUser = await CurrentUserContext.GetCurrentUserContext();

        //act
        var command = new UnfollowUser.Command("missingUserName");
        var handler = new UnfollowUser.Handler(Mapper, ServiceMgr, _logger, Mediator);
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

        var currentUser = await CurrentUserContext.GetCurrentUserContext();

        //act
        var command = new UnfollowUser.Command(currentUser.UserName);
        var handler = new UnfollowUser.Handler(Mapper, ServiceMgr, _logger, Mediator);
        var result = await handler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.ShouldBeFalse();
        var userFollows = Context.UserFollows.ToList();
        userFollows.Count.ShouldBe(0);
    }
}