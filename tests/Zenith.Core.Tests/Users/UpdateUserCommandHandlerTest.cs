using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Users
{
    [Collection("ZenithCollectionFixture")]
    public class UpdateUserCommandHandlerTest : TestFixture
    {
        private readonly ILogger<UpdateUser.Handler> _logger;

        public UpdateUserCommandHandlerTest()
        {
            _logger = NullLogger<UpdateUser.Handler>.Instance;
        }

        [Fact]
        public async void GivenValidUserRequest_WhenTheUserExistsAndUpdatesEmail_ReturnsUpdateUserViewModelResponseAndIssueNewToken()
        {
            var updateUserCommand = new UpdateUser.Command
            (
                "aUpdatedEmail@gmail.com",
                TestConstants.TestUserName,
                "This is my bio",
                "A new image"
            );

            var originalUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);

            var command = new UpdateUser.Handler(CurrentUserContext, Mapper, UserManager, Context, TokenService, Mediator);
            var result = await command.Handle(updateUserCommand, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.Email.ShouldBe(updateUserCommand.Email);
            result.Value.UserName.ShouldBe(updateUserCommand.Username);
            result.Value.Bio.ShouldBe(updateUserCommand.Bio);
            result.Value.Image.ShouldBe(updateUserCommand.Image);

            result.Value.Token.ShouldNotBe(new CurrentUserContextTest(UserManager).GetCurrentUserToken());
            result.Value.Token.ShouldBe(new TokenServiceTest().CreateToken(originalUser));

            var activityLog = Context.ActivityLogs.FirstOrDefault();
            activityLog.ShouldNotBeNull();
            activityLog.ActivityType.ShouldBe(ActivityType.UserUpdated);
            activityLog.TransactionId.ShouldBe(originalUser.Id.ToString());
            activityLog.TransactionType.ShouldBe(TransactionType.ZenithUser);

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUserExistsAndUpdatesUsername_ReturnsUpdateUserViewModelResponseAndIssuesNewToken()
        {
            var updateUserCommand = new UpdateUser.Command
            (
               Username: "aupdatedUsername",
               Bio: "my bio",
               Image: "a new image"
            );

            var originalUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);

            var command = new UpdateUser.Handler(
                CurrentUserContext,
                Mapper,
                UserManager,
                Context,
                TokenService,
                Mediator
               );

            var response = await command.Handle(updateUserCommand, CancellationToken.None);

            response.Value.ShouldNotBeNull();
            response.Value.Email.ShouldBe(TestConstants.TestUserEmail);
            response.Value.UserName.ShouldBe(updateUserCommand.Username);
            response.Value.Bio.ShouldBe(updateUserCommand.Bio);
            response.Value.Image.ShouldBe(updateUserCommand.Image);

            response.Value.Token.ShouldNotBe(new CurrentUserContextTest(UserManager).GetCurrentUserToken());
            response.Value.Token.ShouldBe(new TokenServiceTest().CreateToken(originalUser));

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUserExistsAndDoesNotUpdateUsernameOrEmail_ReturnsUpdateUserViewModelResponseWithSameToken()
        {
            var updateUserCommand = new UpdateUser.Command(Bio: "My Bio", Image: "a new image");

            var originalUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);

            var handler = new UpdateUser.Handler(
                CurrentUserContext,
                Mapper,
                UserManager,
                Context,
                TokenService,
                Mediator
                );

            var response = await handler.Handle(updateUserCommand, CancellationToken.None);

            response.Value.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.Value.Email.ShouldBe(TestConstants.TestUserEmail);
            response.Value.UserName.ShouldBe(TestConstants.TestUserName);
            response.Value.Bio.ShouldBe(updateUserCommand.Bio);
            response.Value.Image.ShouldBe(updateUserCommand.Image);

            response.Value.Token.ShouldBe(new CurrentUserContextTest(UserManager).GetCurrentUserToken());
            response.Value.Token.ShouldNotBe(new TokenServiceTest().CreateToken(originalUser));

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUpdatedUsernameAlreadyExists_ReturnsErrorResponse()
        {
            var updateUserCommand = new UpdateUser.Command(Username: "existing.user");

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = "existingUser@gmail.com",
                NormalizedEmail = "existingUser@gmail.com".ToUpperInvariant(),
                UserName = updateUserCommand.Username,
                NormalizedUserName = updateUserCommand.Username.ToUpperInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
        .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);


            var command = new UpdateUser.Handler(
                CurrentUserContext,
                Mapper,
                UserManager,
                Context,
                TokenService, Mediator);

            var response = await command.Handle(updateUserCommand, CancellationToken.None);

            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldContain($"Username {updateUserCommand.Username} is already in use");
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUpdatedEmailAlreadyExists_ReturnsErrorResponse()
        {
            var updateUserCommand = new UpdateUser.Command(Email: "existingUser@gmail.com");

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = "existingUser@gmail.com",
                NormalizedEmail = "existingUser@gmail.com".ToUpperInvariant(),
                UserName = "existing.user",
                NormalizedUserName = "existing.user".ToUpperInvariant(),
                SecurityStamp = "someRandomSecurityStamp"
            };

            existingUserWithSameUsername.PasswordHash = new PasswordHasher<ZenithUser>()
        .HashPassword(existingUserWithSameUsername, "password");

            await UserManager.CreateAsync(existingUserWithSameUsername);


            var command = new UpdateUser.Handler(
                CurrentUserContext,
                Mapper,
                UserManager,
                Context,
                TokenService, Mediator);

            var response = await command.Handle(updateUserCommand, CancellationToken.None);

            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldContain($"Email {updateUserCommand.Email} is already in use");
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenRequestContainsNewPassword_ReturnsSuccessResponse()
        {
            var updateUserCommand = new UpdateUser.Command(Password: "aNewPassword!124#");

            var originalUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);

            var handler = new UpdateUser.Handler(
                CurrentUserContext,
                Mapper,
                UserManager,
                Context,
                TokenService, Mediator);

            var response = await handler.Handle(updateUserCommand, CancellationToken.None);

            response.IsSuccess.ShouldBeTrue();
            response.Errors.ShouldBeEmpty();
            response.Value.ShouldNotBeNull();
            response.Value.ShouldBeOfType<UserViewModel>();

            var updatedUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);
            updatedUser.PasswordHash.ShouldNotBeNullOrWhiteSpace();

            var validatedPassword = await UserManager.CheckPasswordAsync(originalUser, updateUserCommand.Password);
            var invalidOldPassword = await UserManager.CheckPasswordAsync(originalUser, "#password1!");

            validatedPassword.ShouldBeTrue();
            invalidOldPassword.ShouldBeFalse();
        }
    }
}

