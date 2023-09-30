using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Tests.Infrastructure;
using static Zenith.Core.Features.Articles.CreateArticle;
using Azure;
using Shouldly;
using Zenith.Common.Extensions;

namespace Zenith.Core.Tests.Articles
{
    public class CreateArticleCommandHandlerTest:TestFixture
    {
        private readonly NullLogger<Handler> _logger;

        public CreateArticleCommandHandlerTest()
        {
            _logger = NullLogger<Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidArticleRequest_WhenArticleDoesNotExist_ReturnsSuccessResponseWithViewModel()
        {
            //arrange 
            var articleDto = new CreateArticleDto
            {
                Title = "Test Article",
                Body = "Test Body",
                Description = "Test Description",
                TagList = new List<string> { "Test Tag" }
            };

            //act
            var handler = new Handler(ServiceMgr, Mapper, CurrentUserContext, _logger);
            var result = await handler.Handle(new Command(articleDto), CancellationToken.None);

            //assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.Title.ShouldBe(articleDto.Title);
            result.Value.Slug.ShouldBe(articleDto.Title.ToSlug());
            result.Value.Body.ShouldBe(articleDto.Body);
            result.Value.Description.ShouldBe(articleDto.Description);
            result.Value.Tags.ShouldBe(articleDto.TagList);
            result.Value.Author.UserName.ShouldBe(TestConstants.TestUserName);
            result.Value.FavoriteCount.ShouldBe(0);
            result.Value.Favorited.ShouldBeFalse();
            result.Value.Following.ShouldBeFalse();
        }
    }
}
