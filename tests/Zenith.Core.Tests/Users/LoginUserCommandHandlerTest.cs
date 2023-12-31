﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Zenith.Common.Exceptions;
using Zenith.Core.Domain.Entities;
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
            var command = new LoginUser.Command(
                new Features.Users.Dtos.LoginUserDto
                {
                    Email="test.user@gmail.com", 
                    Password ="#passwordTest1!"
                }                
                );

            var handler = new LoginUser.Handler(UserManager, TokenService, _logger, Mapper, Mediator);
            var response = await handler.Handle(command, CancellationToken.None);

            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.Email.ShouldBe(command.LoginUserDto.Email);
            response.Value.UserName.ShouldBe("test.user");

            var activityLog = Context.ActivityLogs.FirstOrDefault();
            activityLog.ShouldNotBeNull();
            activityLog.ActivityType.ShouldBe(ActivityType.Login);
            
            var user = Context.Users.FirstOrDefault(u => u.UserName == "test.user");
            activityLog.TransactionId.ShouldBe(user.Id);
            activityLog.TransactionType.ShouldBe(TransactionType.ZenithUser);
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenTheUserDoesNotExist_ReturnError()
        {
            var command = new LoginUser.Command(
                new Features.Users.Dtos.LoginUserDto
                {
                    Email = "test.user3@gmail.com",
                    Password = "#passwordTest1!"
                }
            );
            
                var handler = new LoginUser.Handler(UserManager, TokenService, _logger, Mapper, Mediator);
                handler.Handle(command, CancellationToken.None).ShouldThrow<ApiException>()
                    .Message.ShouldBe("Incorrect username or password");
        }

        [Fact]
        public async Task GivenValidUserRequest_WhenThePasswordDoesNotMatch_ReturnsError()
        {
            var command = new LoginUser.Command(
                new Features.Users.Dtos.LoginUserDto
                {
                    Email = "test.user@gmail.com",
                    Password = "#passwordTest2!"
                }
            );
           
            var handler = new LoginUser.Handler(UserManager, TokenService, _logger, Mapper, Mediator);
            handler.Handle(command, CancellationToken.None).ShouldThrow<ApiException>()
                .Message.ShouldBe("Incorrect username or password");
        }
    }
}
