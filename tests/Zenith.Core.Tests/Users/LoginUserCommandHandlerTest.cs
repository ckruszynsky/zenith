﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Core.Features.Users;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Users
{
    [Collection("ZenithCollectionFixture")]
    public class LoginUserCommandHandlerTest : TestFixture
    {
        private readonly ILogger<LoginUser.Handler> _logger;

        public LoginUserCommandHandlerTest()
        {
            _logger = NullLogger<LoginUser.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheUserExists_ReturnsProperUserViewModelResponse()
        {
            var command = new LoginUser.Command("test.user@gmail.com", "#passwordTest1!");

            var handler = new LoginUser.Handler(UserManager, Context, TokenService, _logger, Mapper);
            var response = await handler.Handle(command, CancellationToken.None);

            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.Email.ShouldBe(command.Email);
            response.Value.UserName.ShouldBe("test.user");
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUserDoesNotExist_ReturnError()
        {
            var command = new LoginUser.Command("test.user3@gmail.com", "#passwordTest1!");

            var handler = new LoginUser.Handler(UserManager, Context, TokenService, _logger, Mapper);
            var response = await handler.Handle(command, CancellationToken.None);

            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldNotBeEmpty();
            response.Errors.ShouldContain("Incorrect user or password");

        }

        [Fact]
        public async Task GivenValidUserRequest_WhenThePasswordDoesNotMatch_ReturnsError()
        {
            var command = new LoginUser.Command("test.user@gmail.com", "#passwordTest2!");

            var handler = new LoginUser.Handler(UserManager, Context, TokenService, _logger, Mapper);
            var response = await handler.Handle(command, CancellationToken.None);

            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldNotBeEmpty();
            response.Errors.ShouldContain("Incorrect user or password");
        }
    }
}