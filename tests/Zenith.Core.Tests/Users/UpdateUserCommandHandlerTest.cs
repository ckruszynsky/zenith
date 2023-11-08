using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users;
using Zenith.Core.Features.Users.Dtos;
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
            var updateUserCommand = new UpdateUser.Command (
            
                new UpdateUserDto {
                Email = "aUpdatedEmail@gmail.com",
                Username = TestConstants.TestUserName,
                Bio = "This is my bio",
                Image = "A new image"
                }
            );

            var originalUser = await UserManager.FindByEmailAsync(TestConstants.TestUserEmail);

            var command = new UpdateUser.Handler(CurrentUserContext, Mapper, UserManager, Context, TokenService, Mediator);
            var result = await command.Handle(updateUserCommand, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.Email.ShouldBe(updateUserCommand.UpdateUserDto.Email);
            result.Value.UserName.ShouldBe(updateUserCommand.UpdateUserDto.Username);
            result.Value.Bio.ShouldBe(updateUserCommand.UpdateUserDto.Bio);
            result.Value.Image.ShouldBe(updateUserCommand.UpdateUserDto.Image);

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
            var updateUserCommand = new UpdateUser.Command(

                new UpdateUserDto
                {                    
                    Username = "updatedUserName",
                    Bio = "This is my bio",
                    Image = "A new image"
                }
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
            response.Value.UserName.ShouldBe(updateUserCommand.UpdateUserDto.Username);
            response.Value.Bio.ShouldBe(updateUserCommand.UpdateUserDto.Bio);
            response.Value.Image.ShouldBe(updateUserCommand.UpdateUserDto.Image);

            response.Value.Token.ShouldNotBe(new CurrentUserContextTest(UserManager).GetCurrentUserToken());
            response.Value.Token.ShouldBe(new TokenServiceTest().CreateToken(originalUser));

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUserExistsAndDoesNotUpdateUsernameOrEmail_ReturnsUpdateUserViewModelResponseWithSameToken()
        {
            var updateUserCommand = new UpdateUser.Command(

                new UpdateUserDto
                {
                    Bio = "This is my bio",
                    Image = "A new image"
                }
            );

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
            response.Value.Bio.ShouldBe(updateUserCommand.UpdateUserDto.Bio);
            response.Value.Image.ShouldBe(updateUserCommand.UpdateUserDto.Image);

            response.Value.Token.ShouldBe(new CurrentUserContextTest(UserManager).GetCurrentUserToken());
            response.Value.Token.ShouldNotBe(new TokenServiceTest().CreateToken(originalUser));

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUpdatedUsernameAlreadyExists_ReturnsErrorResponse()
        {
            var updateUserCommand = new UpdateUser.Command(

                new UpdateUserDto
                {                   
                    Username = "updatedUserName"                 
                }
            );

            var existingUserWithSameUsername = new ZenithUser
            {
                Email = "existingUser@gmail.com",
                NormalizedEmail = "existingUser@gmail.com".ToUpperInvariant(),
                UserName = updateUserCommand.UpdateUserDto.Username,
                NormalizedUserName = updateUserCommand.UpdateUserDto.Username.ToUpperInvariant(),
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

            command.Handle(updateUserCommand, CancellationToken.None)
                .ShouldThrow<ApiException>()
                .Message.ShouldBe($"Username {updateUserCommand.UpdateUserDto.Username} is already in use");            
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUpdatedEmailAlreadyExists_ReturnsErrorResponse()
        {
            var updateUserCommand = new UpdateUser.Command(

                new UpdateUserDto
                {
                    Email = "existingUser@gmail.com",                    
                }
            );

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

             command.Handle(updateUserCommand, CancellationToken.None)
                .ShouldThrow<ApiException>()
                .Message.ShouldBe($"Email {updateUserCommand.UpdateUserDto.Email} is already in use");

           
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenRequestContainsNewPassword_ReturnsSuccessResponse()
        {
            var updateUserCommand = new UpdateUser.Command(

                new UpdateUserDto
                {
                   Password = "newPassword"
                }
            );

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

            var validatedPassword = await UserManager.CheckPasswordAsync(originalUser, updateUserCommand.UpdateUserDto.Password);
            var invalidOldPassword = await UserManager.CheckPasswordAsync(originalUser, "#password1!");

            validatedPassword.ShouldBeTrue();
            invalidOldPassword.ShouldBeFalse();
        }
    }
}

