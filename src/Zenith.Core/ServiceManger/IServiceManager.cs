using Zenith.Core.Features.ActivityLog.Contracts;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Features.Profile.Contracts;
using Zenith.Core.Features.Tags.Contracts;

namespace Zenith.Core.ServiceManger
{
    public interface IServiceManager
    {
        IArticleService Articles { get; }
        ITagService Tags { get; }
        IActivityLogService ActivityLogs { get; }
        IProfileService Profiles { get; }

        Task SaveAsync();
    }
}
