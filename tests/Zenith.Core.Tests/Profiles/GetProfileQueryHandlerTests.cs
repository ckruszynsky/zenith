using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Zenith.Core.Features.Profile;
using Zenith.Core.Features.Profile.ViewModel;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Profiles
{
    public class GetProfileQueryHandlerTests:TestFixture
    {
        private readonly NullLogger<GetProfile.Handler> _logger;

        public GetProfileQueryHandlerTests()
        {
            _logger = NullLogger<GetProfile.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheUserExists_ReturnsProfileViewModel()
        {
            //arrange
            var query = new GetProfile.Query(TestConstants.TestUserName);
            var handler = new GetProfile.Handler(Mapper, _logger, ServiceMgr);

            var response = await handler.Handle(query, CancellationToken.None);
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldBeOfType<ProfileViewModel>();
            response.Value.UserName.ShouldBe(TestConstants.TestUserName);
            response.Value.Following.ShouldBeFalse();
            
        }

        [Fact]
        public async Task GivenValidRequest_WhenTheUserDoesntExists_ReturnsErrorResult()
        {
            //arrange
            var query = new GetProfile.Query("fakeUser");
            var handler = new GetProfile.Handler(Mapper, _logger, ServiceMgr);

            var response = await handler.Handle(query, CancellationToken.None);
            response.IsSuccess.ShouldBeFalse();
            response.Errors.Count().ShouldBe(1);

        }

    }
}
