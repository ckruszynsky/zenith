using Zenith.Core.Features.Articles;

namespace Zenith.Core.ServiceManger
{
    public interface IServiceManager
    {
        IArticleService Article { get; }
        Task SaveAsync();
    }
}
