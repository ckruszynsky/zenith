using System.Net;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenith.Common.Responses;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Articles.Models;

namespace Zenith.API.Controllers
{
    [Authorize]
    public class ArticlesController:ZenithControllerBase
    {
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ILogger<ArticlesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]        
        [ProducesResponseType(typeof(PaginatedList<ArticleFeedViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedList<ArticleFeedViewModel>>> GetArticles([FromQuery] ArticleFeedDto feedParameters)
        {
            var result = await Mediator.Send(new GetArticlesFeed.Query(feedParameters));
            return result.ToActionResult(this);
        }
    }
}
