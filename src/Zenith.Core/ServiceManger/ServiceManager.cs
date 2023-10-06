using AutoMapper;
using Zenith.Core.Features.ActivityLog;
using Zenith.Core.Features.ActivityLog.Contracts;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Features.Profile;
using Zenith.Core.Features.Profile.Contracts;
using Zenith.Core.Features.Tags;
using Zenith.Core.Features.Tags.Contracts;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.ServiceManger
{
    public class ServiceManager : IServiceManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUserContext _currentUserContext;
        private IArticleService? _articleService = null;
        private ITagService? _tagService = null;
        private IActivityLogService? _activityLogService = null;
        private IProfileService _profileService = null;

        public ServiceManager(AppDbContext appDbContext, IMapper mapper, ICurrentUserContext currentUserContext)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _currentUserContext = currentUserContext;
        }

        public IArticleService Articles
        {
            get
            {
                _articleService ??= new ArticleService(_appDbContext, _mapper);
                return _articleService;
            }
        }

        public ITagService Tags
        {
            get
            {
                _tagService ??= new TagService(_appDbContext, _mapper);
                return _tagService;
            }
        }

        public IActivityLogService ActivityLogs {
            get
            {
                _activityLogService ??= new ActivityLogService(_appDbContext);
                return _activityLogService;
            }
        }

        public IProfileService Profiles
        {
            get
            {
                _profileService ??= new ProfileService(_appDbContext, _currentUserContext);
                return _profileService;
            }
        }

        public Task SaveAsync()
        {
            return _appDbContext.SaveChangesAsync();
        }
    }
}
