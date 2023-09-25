using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
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
    }
}
