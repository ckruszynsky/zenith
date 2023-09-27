using Zenith.Core.Features.Articles.Contracts;

namespace Zenith.Core.ServiceManger
{
    public interface IServiceManager
    {
        IArticleService Article { get; }
        Task SaveAsync();
    }
}
