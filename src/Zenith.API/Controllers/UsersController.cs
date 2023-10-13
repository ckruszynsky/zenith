using System.Net;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenith.Core.Features.Users;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.API.Controllers
{
    public class UsersController:ZenithControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]       
        public async Task<ActionResult<UserViewModel>> Register([FromBody] CreateUserDto createUserDto)
        {
            var result = await Mediator.Send(new CreateUser.Command(createUserDto));
            return result.ToActionResult(this);            
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserViewModel>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await Mediator.Send(new LoginUser.Command(loginUserDto));
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UserViewModel>> Update([FromBody] UpdateUserDto updateUserDto)
        {
            var result = await Mediator.Send(new UpdateUser.Command(updateUserDto));
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var result = await Mediator.Send(new LogoutUser.Command());
            return Ok();
        }


    }
}
