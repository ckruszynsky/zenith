using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Zenith.Common.Exceptions;

namespace Zenith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZenithControllerBase:ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= (IMediator)HttpContext.RequestServices.GetService(typeof(IMediator));
        
        protected ActionResult<TResult> HandleResult<TResult>(Result<TResult> result) where TResult : class
        {
            if (result.IsSuccess)
            {
                return result.ToActionResult(this);
            }
            
            throw new ApiException(string.Join(',', result.Errors), HttpStatusCode.BadRequest);
        }

    }
}
