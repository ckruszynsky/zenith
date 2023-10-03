using Zenith.Core.Features.ActivityLog.Contracts;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Features.Tags.Contracts;

namespace Zenith.Core.ServiceManger
{
    public interface IServiceManager
    {
        IArticleService Articles { get; }
        ITagService Tags { get; }
        IActivityLogService ActivityLogs { get; }
        Task SaveAsync();
    }
}
