using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Domain.ViewModels;
using Zenith.Core.Features.Users;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests.Users
{
    using static TestingFixture;
    public class RegisterUsersControllerTests : BaseTestFixture
    {
        [Test]
        public async Task GivenAUserCreateRequest_WhenRequestIsValid_ReturnsUserViewModelWithSuccessfulResponse()
        {
            //Arrange
            var newUserRequest = new CreateUserDto
            {
                Email = "a-brand-new-user@gmail.com",
                Username = "a-brand-new-user",
                Password = "#password1!",
            };

            var requestContent = ContentHelper.GetRequestContent(newUserRequest);

            var client = GetHttpClient();

            //Act
            var response = await client.PostAsync(RegisterEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<UserViewModel>(response);


            //Assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<UserViewModel>();
            responseContent.Email.ShouldBe(newUserRequest.Email);
            responseContent.UserName.ShouldBe(newUserRequest.Username);
            responseContent.Token.ShouldNotBeNullOrEmpty();

        }

        [Test]
        public async Task GivenAUserCreateRequest_WhenUserAlreadyExists_ReturnsErrorResponse()
        {

            //Arrange
            var newUserRequest = new CreateUserDto
            {
                Email = "a-brand-new-user@gmail.com",
                Username = "a-brand-new-user",
                Password = "#password1!",
            };

            await AddAsync<ZenithUser>(new ZenithUser
            {
                Email = newUserRequest.Email,
                UserName = newUserRequest.Username,
                NormalizedEmail = newUserRequest.Email.ToUpperInvariant(),
                NormalizedUserName = newUserRequest.Username.ToUpperInvariant(),                
            });

            var requestContent = ContentHelper.GetRequestContent(newUserRequest);
            var client = GetHttpClient();

            //act
            var response = await client.PostAsync(RegisterEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.ShouldNotBeNull();
            responseContent.Errors.ShouldNotBeNull();
        }

        [Test]
        public async Task GivenAUserCreateRequest_WhenUserAlreadyExistsWithEmail_ReturnsErrorResponse()
        {

            //Arrange
            var newUserRequest = new CreateUserDto
            {
                Email = "a-brand-new-user@gmail.com",
                Username = "a-brand-new-user-1",
                Password = "#password1!",
            };

            await AddAsync<ZenithUser>(new ZenithUser
            {
                Email = newUserRequest.Email,
                UserName = "some-user",
                NormalizedEmail = newUserRequest.Email.ToUpperInvariant(),
                NormalizedUserName = "some-user".ToUpperInvariant(),    
            });

            var requestContent = ContentHelper.GetRequestContent(newUserRequest);
            var client = GetHttpClient();

            //act
            var response = await client.PostAsync(RegisterEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.ShouldNotBeNull();
            responseContent.Errors.ShouldNotBeNull();
        }

        [Test]
        public async Task GivenAUserCreateRequest_WhenRequestIsMissingProperties_ReturnsErrorResponse()
        {

            //Arrange
            var newUserRequest = new CreateUserDto
            {                
                Username = "a-brand-new-user",
                Password = "#password1!",
            };
            
            var requestContent = ContentHelper.GetRequestContent(newUserRequest);
            var client = GetHttpClient();

            //act
            var response = await client.PostAsync(RegisterEndpoint, requestContent);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnsupportedMediaType);
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.ShouldNotBeNull();
            responseContent.Errors.ShouldNotBeNull();
        }
    }
}
