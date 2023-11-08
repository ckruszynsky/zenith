using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Domain.ViewModels;
using Zenith.Core.Features.Users;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests.Users
{
    using static TestingFixture;
    public class UpdateUserControllerTest:BaseTestFixture
    {

        [Test]
        public async Task GivenValidUserUpdateRequest_WhenUserIsFoundTheEmailIsUpdated_ReturnsUserViewModel()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "aUser@gmail.com",
                NormalizedEmail = "aUser@gmail.com".ToUpperInvariant(),
                UserName = "aUser",
                NormalizedUserName = "aUser".ToUpperInvariant(),
            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);

            var loginRequest = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password1!",
            };
            var client = GetHttpClient();

            await ContentHelper.GetRequestWithAuthorization(client, loginRequest);
            var request = ContentHelper.GetRequestContent(loginRequest);
            var token = client.DefaultRequestHeaders.Authorization.ToString().Split(" ")[1];

            var updateUserRequest = new UpdateUserDto
            {
                Email = "aUpdatedUser@gmail.com"
            };

            var updateRequest = ContentHelper.GetRequestContent(updateUserRequest);

            //act 
            var response = await client.PutAsync(UpdateUserEndpoint, updateRequest);
            var responseContent = await ContentHelper.GetResponseContent<UserViewModel>(response);
            
            //assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<UserViewModel>();
            responseContent.Email.ShouldBe(updateUserRequest.Email);

            //validate new token issued
            responseContent.Token.ShouldNotBeNullOrWhiteSpace();
            responseContent.Token.ShouldNotBe(token);

            //validate user can login with new email
            var loginRequestWithUpdatedEmail = new LoginUserDto
            {
                Email = updateUserRequest.Email,
                Password = "#password1!",
            };

            var requestWithUpdatedEmail = ContentHelper.GetRequestContent(loginRequestWithUpdatedEmail);
            var responseWithUpdatedEmail = await client.PostAsync(LoginEndpoint, requestWithUpdatedEmail);
            var responseContentWithUpdatedEmail = await ContentHelper.GetResponseContent<UserViewModel>(responseWithUpdatedEmail);
            
            responseWithUpdatedEmail.EnsureSuccessStatusCode();
            responseContentWithUpdatedEmail.ShouldNotBeNull();
            responseContentWithUpdatedEmail.ShouldBeOfType<UserViewModel>();
            responseContentWithUpdatedEmail.Email.ShouldBe(updateUserRequest.Email);
            responseContentWithUpdatedEmail.Token.ShouldNotBeNullOrWhiteSpace();            
        }

        [Test]
        public async Task GivenValidUserUpdateRequest_WhenUserIsFoundTheUsernameIsUpdated_ReturnsUserViewModel()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "aUser@gmail.com",
                NormalizedEmail = "aUser@gmail.com".ToUpperInvariant(),
                UserName = "aUser",
                NormalizedUserName = "aUser".ToUpperInvariant(),
            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);

            var loginRequest = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password1!",
            };
            var client = GetHttpClient();

            await ContentHelper.GetRequestWithAuthorization(client, loginRequest);
            var request = ContentHelper.GetRequestContent(loginRequest);
            var token = client.DefaultRequestHeaders.Authorization.ToString().Split(" ")[1];

            var updateUserRequest = new UpdateUserDto
            {
                Username = "aUpdatedUser"
            };

            var updateRequest = ContentHelper.GetRequestContent(updateUserRequest);

            //act 
            var response = await client.PutAsync(UpdateUserEndpoint, updateRequest);
            var responseContent = await ContentHelper.GetResponseContent<UserViewModel>(response);

            //assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<UserViewModel>();
            responseContent.UserName.ShouldBe(updateUserRequest.Username);

            //validate new token issued
            responseContent.Token.ShouldNotBeNullOrWhiteSpace();
            responseContent.Token.ShouldNotBe(token);
          
        }

        [Test]
        public async Task GivenValidUserUpdateRequest_WhenUserIsFoundThePasswordIsUpdated_ReturnsUserViewModel()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "aUser@gmail.com",
                NormalizedEmail = "aUser@gmail.com".ToUpperInvariant(),
                UserName = "aUser",
                NormalizedUserName = "aUser".ToUpperInvariant(),
            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            await AddAsync<ZenithUser>(testUser);

            var loginRequest = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password1!",
            };
            var client = GetHttpClient();

            await ContentHelper.GetRequestWithAuthorization(client, loginRequest);
            var request = ContentHelper.GetRequestContent(loginRequest);
            var token = client.DefaultRequestHeaders.Authorization.ToString().Split(" ")[1];

            var updateUserRequest = new UpdateUserDto
            {
                Password = "#password2!"
            };

            var updateRequest = ContentHelper.GetRequestContent(updateUserRequest);

            //act 
            var response = await client.PutAsync(UpdateUserEndpoint, updateRequest);
            var responseContent = await ContentHelper.GetResponseContent<UserViewModel>(response);

            //assert
            response.EnsureSuccessStatusCode();
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<UserViewModel>();         

            //validate new token issued
            responseContent.Token.ShouldNotBeNullOrWhiteSpace();
            responseContent.Token.ShouldBe(token);
            
            //validate user can login with new password
            var loginRequestWithUpdatedEmail = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password2!",
            };

            var requestWithUpdatedEmail = ContentHelper.GetRequestContent(loginRequestWithUpdatedEmail);
            var responseWithUpdatedEmail = await client.PostAsync(LoginEndpoint, requestWithUpdatedEmail);
            var responseContentWithUpdatedEmail = await ContentHelper.GetResponseContent<UserViewModel>(responseWithUpdatedEmail);

            responseWithUpdatedEmail.EnsureSuccessStatusCode();
            responseContentWithUpdatedEmail.ShouldNotBeNull();
            responseContentWithUpdatedEmail.ShouldBeOfType<UserViewModel>();            
            responseContentWithUpdatedEmail.Token.ShouldNotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task GivenValidUserUpdateRequest_WhenEmailIsUpdatedToAlreadyExisitingEmail_ReturnsErrorViewModelWithBadRequest()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "aUser@gmail.com",
                NormalizedEmail = "aUser@gmail.com".ToUpperInvariant(),
                UserName = "aUser",
                NormalizedUserName = "aUser".ToUpperInvariant(),
            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            var testUser2 = new ZenithUser
            {
                Email = "aUser2@gmail.com",
                NormalizedEmail = "aUser2@gmail.com".ToUpperInvariant(),
                UserName = "aUser2",
                NormalizedUserName = "aUser2".ToUpperInvariant(),
            };
            testUser2.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser2, "#password1!");

            await AddAsync<ZenithUser>(testUser);
            await AddAsync<ZenithUser>(testUser2);

            var loginRequest = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password1!",
            };
            var client = GetHttpClient();

            await ContentHelper.GetRequestWithAuthorization(client, loginRequest);
            var request = ContentHelper.GetRequestContent(loginRequest);          
            var updateUserRequest = new UpdateUserDto
            {
                Email = "aUser2@gmail.com"
            };

            var updateRequest = ContentHelper.GetRequestContent(updateUserRequest);

            //act 
            var response = await client.PutAsync(UpdateUserEndpoint, updateRequest);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.Errors.ShouldNotBeNull();

        }

        [Test]
        public async Task GivenValidUserUpdateRequest_WhenUsernameAlreadyExists_ReturnsErrorViewModelWithBadRequest()
        {
            //arrange
            var testUser = new ZenithUser
            {
                Email = "aUser@gmail.com",
                NormalizedEmail = "aUser@gmail.com".ToUpperInvariant(),
                UserName = "aUser",
                NormalizedUserName = "aUser".ToUpperInvariant(),
            };
            testUser.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser, "#password1!");

            var testUser2 = new ZenithUser
            {
                Email = "aUser2@gmail.com",
                NormalizedEmail = "aUser2@gmail.com".ToUpperInvariant(),
                UserName = "aUser2",
                NormalizedUserName = "aUser2".ToUpperInvariant(),
            };
            testUser2.PasswordHash = new PasswordHasher<ZenithUser>()
                .HashPassword(testUser2, "#password1!");

            await AddAsync<ZenithUser>(testUser);
            await AddAsync<ZenithUser>(testUser2);

            var loginRequest = new LoginUserDto
            {
                Email = "aUser@gmail.com",
                Password = "#password1!",
            };
            var client = GetHttpClient();

            await ContentHelper.GetRequestWithAuthorization(client, loginRequest);
            var request = ContentHelper.GetRequestContent(loginRequest);
            var updateUserRequest = new UpdateUserDto
            {
                Username = "aUser2"
            };

            var updateRequest = ContentHelper.GetRequestContent(updateUserRequest);

            //act 
            var response = await client.PutAsync(UpdateUserEndpoint, updateRequest);
            var responseContent = await ContentHelper.GetResponseContent<ErrorViewModel>(response);

            //assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            responseContent.ShouldNotBeNull();
            responseContent.ShouldBeOfType<ErrorViewModel>();
            responseContent.Errors.ShouldNotBeNull();
        }
    }
}
