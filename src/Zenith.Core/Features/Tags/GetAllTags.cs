using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.Articles.Dtos;
using Zenith.Core.Features.Tags.Dtos;
using Zenith.Core.Features.Tags.ViewModels;
using Zenith.Core.ServiceManger;

namespace Zenith.Core.Features.Tags
{
    public class GetAllTags
    {
        public record Query(int PageSize, int CurrentPage):IRequest<PagedResult<IEnumerable<TagViewModel>>>;

        public class Handler : IRequestHandler<Query, PagedResult<IEnumerable<TagViewModel>>>
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
            public async Task<PagedResult<IEnumerable<TagViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tagListDto = await _serviceManager.Tags.GetAllTagsAsync(request.CurrentPage,request.PageSize);                
                var pageCount = (int)Math.Ceiling((double)tagListDto.TotalCount / request.PageSize);
                var pagedInfo = new PagedInfo(request.CurrentPage, request.PageSize, pageCount,
                    tagListDto.TotalCount);

                var tagViewModels = _mapper.Map<IEnumerable<TagViewModel>>(tagListDto.Tags);

                return Result<IEnumerable<TagViewModel>>.Success(tagViewModels).ToPagedResult(pagedInfo);
            }
        }
    }
}
