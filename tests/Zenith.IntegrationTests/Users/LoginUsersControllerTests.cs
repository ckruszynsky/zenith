using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Domain.ViewModels;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests.Users
{
    using static TestingFixture;
    public class LoginUsersControllerTests:BaseTestFixture
    {
        [Test]
        public async Task GivenAUserLoginRequest_WhenUserNameAndPasswordIsValid_ReturnsLoginViewModelAndSuccess()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "someUser2@gmail.com",
                UserName = "someUser2",
                NormalizedEmail = "someUser2@gmail.com".ToUpperInvariant(),
                NormalizedUserName = "someUser2".ToUpperInvariant(),

            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);
           
            var loginRequest = new LoginUserDto
            {
                Email = "someUser2@gmail.com",
                Password = "#password1!",
            };
            
            var client = GetHttpClient();

            var requestContent = ContentHelper.GetRequestContent(loginRequest);

            //act
            var response = await client.PostAsync(LoginEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<LoginUserDto>(response);

            //assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<LoginUserDto>();
        }

        [Test]
        public async Task GivenAUserLoginRequest_WhenUserNameAndPasswordIsInValid_ReturnsErrorViewModelWithBadRequest()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "someUser2@gmail.com",
                UserName = "someUser2",
                NormalizedEmail = "someUser2@gmail.com".ToUpperInvariant(),
                NormalizedUserName = "someUser2".ToUpperInvariant(),

            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);

            var loginRequest = new LoginUserDto
            {
                Email = "someUser2@gmail.com",
                Password = "#TestPassword1!",
            };

            var client = GetHttpClient();

            var requestContent = ContentHelper.GetRequestContent(loginRequest);

            //act
            var response = await client.PostAsync(LoginEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.Errors.ShouldNotBeNull();
        }

        [Test]
        public async Task GivenAUserLoginRequest_WhenUserDoesNotExist_ReturnsErrorViewModelWithBadRequest()
        {
           //arrange
            var loginRequest = new LoginUserDto
            {
                Email = "someUser2@gmail.com",
                Password = "#TestPassword1!",
            };

            var client = GetHttpClient();

            var requestContent = ContentHelper.GetRequestContent(loginRequest);

            //act
            var response = await client.PostAsync(LoginEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.Errors.ShouldNotBeNull();
        }

        [Test]
        public async Task GivenAUserLoginRequest_WhenRequestIsInvalid_ReturnsErrorViewModelWithUnsupportedMediaType()
        {
            //arrange
            var loginRequest = new LoginUserDto
            {
                Email = "someUser@gmail.com"
            };

            var requestContent = ContentHelper.GetRequestContent(loginRequest);

            var client = GetHttpClient();

            //act
            var response = await client.PostAsync(LoginEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnsupportedMediaType);
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.Errors.ShouldNotBeNull();
        }
    }
}
