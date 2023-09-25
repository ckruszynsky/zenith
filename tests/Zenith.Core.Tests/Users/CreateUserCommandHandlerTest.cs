using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Users
{
    [Collection("ZenithCollectionFixture")]
    public class CreateUserCommandHandlerTest : TestFixture
    {
        private readonly NullLogger<CreateUser.Handler> _logger;

        public CreateUserCommandHandlerTest()
        {
            _logger = NullLogger<CreateUser.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidUserCommand_WhenUserNameAndEmailAreUnique_ShouldReturnSuccess()
        {
            var createUserCommand = new CreateUser.Command(
                "aTestUserName",
                "aTestEmail@gmail.com",
                "#testPassword1!");


            var command = new CreateUser.Handler(_logger, UserManager, Context, Mapper, TokenService);
            var result = await command.Handle(createUserCommand, CancellationToken.None);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<UserViewModel>();
            result.Value.UserName.ShouldBe(createUserCommand.Username);
            result.Value.Email.ShouldBe(createUserCommand.Email);
            Context.Users.Single(u => u.UserName == createUserCommand.Username).ShouldNotBeNull();
        }

        [Fact]
        public async Task GivenRequestContainsExistingEmail_WhenUserAlreadyExists_ReturnsError()
        {
            var createUserCommand = new CreateUser.Command
            (
                "testEmail@gmail.com",
                "testUsername1",
                "#testPassword1!"
            );

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = createUserCommand.Email,
                NormalizedEmail = createUserCommand.Email.ToUpperInvariant(),
                UserName = "testUserName12",
                NormalizedUserName = "testUserName12".ToLowerInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
            .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);

            var command = new CreateUser.Handler(_logger, UserManager, Context, Mapper, TokenService);

            var result = command.Handle(createUserCommand, CancellationToken.None);

            result.Result.IsSuccess.ShouldBeFalse();
            result.Result.Errors.ShouldNotBeEmpty();
            result.Result.Errors.ShouldContain($"Email {createUserCommand.Email} is already in use");
        }

        [Fact]
        public async Task GivenRequestContainsExistingUsername_WhenUserAlreadyExists_ReturnsError()
        {
            var createUserCommand = new CreateUser.Command
            (
                "testEmail@gmail.com",
                "testUsername1",
                "#testPassword1!"
            );

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = "testEmail1@gmail.com",
                NormalizedEmail = "testEmail1@gmail.com".ToUpperInvariant(),
                UserName = createUserCommand.Username,
                NormalizedUserName = createUserCommand.Username.ToLowerInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
            .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);

            var command = new CreateUser.Handler(_logger, UserManager, Context, Mapper, TokenService);

            var result = command.Handle(createUserCommand, CancellationToken.None);

            result.Result.IsSuccess.ShouldBeFalse();
            result.Result.Errors.ShouldNotBeEmpty();
            result.Result.Errors.ShouldContain($"Username {createUserCommand.Username} is already in use");
        }
    }
}
