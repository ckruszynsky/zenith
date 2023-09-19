using Zenith.Core.Features.Articles;
using Zenith.Persistence;

namespace Zenith.Core.ServiceManger
{
    public class ServiceManager : IServiceManager
    {
        private readonly AppDbContext _appDbContext;
        private IArticleService? _articleService = null;

        public ServiceManager(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IArticleService Article
        {
            get
            {
                _articleService ??= new ArticleService(_appDbContext);
                return _articleService;
            }
        }

        public Task SaveAsync()
        {
            return _appDbContext.SaveChangesAsync();
        }
    }
}
