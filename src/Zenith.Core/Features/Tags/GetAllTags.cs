using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Responses;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Tags.Dtos;
using Zenith.Core.Features.Tags.ViewModels;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Tags
{
    public class GetAllTags
    {
        public record Query(int PageSize = 10, int CurrentPage= 1):IRequest<Result<PaginatedList<TagViewModel>>>;

        public class Handler : IRequestHandler<Query, Result<PaginatedList<TagViewModel>>>
        {
            private readonly IMapper _mapper;
            private readonly IServiceManager _serviceManager;
            private readonly ILogger<Handler> _logger;

            public Handler(IMapper mapper, IServiceManager serviceManager, ILogger<Handler> logger)
            {
                _mapper = mapper;
                _serviceManager = serviceManager;
                _logger = logger;
            }
            public async Task<Result<PaginatedList<TagViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tagListDto = await _serviceManager.Tags.GetAllTagsAsync(request.CurrentPage,request.PageSize);                
               
                var tagViewModels = _mapper.Map<IEnumerable<TagViewModel>>(tagListDto.Tags);
                var paginatedList = tagViewModels.ToPagedList(tagListDto.TotalCount,request.CurrentPage,
                                       request.PageSize);

                return Result.Success(paginatedList);
            }
        }
    }
}
