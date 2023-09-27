using AutoMapper;
using Zenith.Core.Features.Articles;
using Zenith.Core.Features.Articles.Contracts;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.Core.ServiceManger
{
    public class ServiceManager : IServiceManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private IArticleService? _articleService = null;

        public ServiceManager(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public IArticleService Article
        {
            get
            {
                _articleService ??= new ArticleService(_appDbContext, _mapper);
                return _articleService;
            }
        }

        public Task SaveAsync()
        {
            return _appDbContext.SaveChangesAsync();
        }
    }
}
