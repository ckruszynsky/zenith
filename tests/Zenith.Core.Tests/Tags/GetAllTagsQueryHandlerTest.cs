using FizzWare.NBuilder;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Tags;
using Zenith.Core.Tests.Infrastructure;

namespace Zenith.Core.Tests.Tags
{
    public class GetAllTagsQueryHandlerTest:TestFixture
    {
        private readonly NullLogger<GetAllTags.Handler> _logger;

        public GetAllTagsQueryHandlerTest()
        {
            _logger = NullLogger<GetAllTags.Handler>.Instance;
        }

        [Fact]
        public async Task GivenValidRequest_WhenMultipleTagsExist_ReturnsSuccessWithListOfTagViewModel()
        {
            // Arrange
            var tagData = Builder<Tag>.CreateListOfSize(10)
                .All()
                .With(t=> t.Name = Faker.Lorem.Words(1).First())
                .Build();

            Context.Tags.AddRange(tagData);
            await Context.SaveChangesAsync();

            // Act
            var query = new GetAllTags.Query(10, 0);
            var handler = new GetAllTags.Handler(Mapper, ServiceMgr, _logger);
            var response = await handler.Handle(query, CancellationToken.None);

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.PageNumber.ShouldBe(0);
            response.PagedInfo.PageSize.ShouldBe(10);
            response.PagedInfo.TotalPages.ShouldBe(1);
            response.PagedInfo.TotalRecords.ShouldBe(10);
        }

        [Fact]
        public async Task GivenValidRequest_WhenNoTagsExist_ReturnsSuccessWithEmptyListOfTagViewModel()
        {
            // Arrange
            // Act
            var query = new GetAllTags.Query(10, 0);
            var handler = new GetAllTags.Handler(Mapper, ServiceMgr, _logger);
            var response = await handler.Handle(query, CancellationToken.None);

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccess.ShouldBeTrue();
            response.PagedInfo.PageNumber.ShouldBe(0);
            response.PagedInfo.PageSize.ShouldBe(10);
            response.PagedInfo.TotalPages.ShouldBe(0);
            response.PagedInfo.TotalRecords.ShouldBe(0);
        }
    }
}
