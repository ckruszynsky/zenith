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
            var createUserCommand = new CreateUser.Command(new Features.Users.Dtos.CreateUserDto
            {
                Email = "aTestEmail@gmail.com",
                Username = "aTestUserName",
                Password = "#testPassword1!"
            });
             


            var command = new CreateUser.Handler(_logger, UserManager,Mapper, TokenService, Mediator);
            var result = await command.Handle(createUserCommand, CancellationToken.None);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<UserViewModel>();
            result.Value.UserName.ShouldBe(createUserCommand.CreateUserDto.Username);
            result.Value.Email.ShouldBe(createUserCommand.CreateUserDto.Email);
            Context.Users.Single(u => u.UserName == createUserCommand.CreateUserDto.Username).ShouldNotBeNull();

            var activityLog = Context.ActivityLogs.FirstOrDefault();
            activityLog.ShouldNotBeNull();  
            activityLog.ActivityType.ShouldBe(ActivityType.UserCreated);            
            var user = Context.Users.FirstOrDefault(u => u.UserName == createUserCommand.CreateUserDto.Username);
            activityLog.TransactionId.ShouldBe(user.Id.ToString());
            activityLog.TransactionType.ShouldBe(TransactionType.ZenithUser);
        }


        [Fact]
        public async Task GivenRequestContainsExistingEmail_WhenUserAlreadyExists_ReturnsError()
        {
            var createUserCommand = new CreateUser.Command(new Features.Users.Dtos.CreateUserDto
            {
                Email = "aTestEmail@gmail.com",
                Username = "aTestUserName",
                Password = "#testPassword1!"
            });

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = createUserCommand.CreateUserDto.Email,
                NormalizedEmail = createUserCommand.CreateUserDto.Email.ToUpperInvariant(),
                UserName = "testUserName12",
                NormalizedUserName = "testUserName12".ToLowerInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
            .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);

            var command = new CreateUser.Handler(_logger, UserManager, Mapper, TokenService, Mediator);

            var response = await command.Handle(createUserCommand, CancellationToken.None);

            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldNotBeEmpty();
            response.Errors.ShouldContain($"Email {createUserCommand.CreateUserDto.Email} is already in use");
        }

        [Fact]
        public async Task GivenRequestContainsExistingUsername_WhenUserAlreadyExists_ReturnsError()
        {
            var createUserCommand = new CreateUser.Command(new Features.Users.Dtos.CreateUserDto
            {
                Email = "aTestEmail@gmail.com",
                Username = "aTestUserName",
                Password = "#testPassword1!"
            });

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = "testEmail1@gmail.com",
                NormalizedEmail = "testEmail1@gmail.com".ToUpperInvariant(),
                UserName = createUserCommand.CreateUserDto.Username,
                NormalizedUserName = createUserCommand.CreateUserDto.Username.ToLowerInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
            .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);

            var command = new CreateUser.Handler(_logger, UserManager, Mapper, TokenService, Mediator);

            var result = await command.Handle(createUserCommand, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldNotBeEmpty();
            result.Errors.ShouldContain($"Username {createUserCommand.CreateUserDto.Username} is already in use");
        }
    }
}
