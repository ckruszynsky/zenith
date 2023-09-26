using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zenith.Common.Date;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Persistence;
using Zenith.Core.ServiceManger;
using Zenith.Core.Tests.Factories;

namespace Zenith.Core.Tests.Infrastructure
{
    public class TestFixture : IDisposable
    {
        protected AppDbContext Context { get; }

        protected UserManager<ZenithUser> UserManager { get; }

        protected IMapper Mapper { get; }

        protected ITokenService TokenService { get; }

        protected ICurrentUserContext CurrentUserContext { get; }

        protected IDateTime MachineDateTime { get; }

        protected IServiceManager ServiceMgr { get; }

        public TestFixture()
        {
            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase($"{Guid.NewGuid().ToString()}.db"));

            services.AddIdentity<ZenithUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddLogging();

            // Configure HTTP context for authentication
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddSingleton<IHttpContextAccessor>(_ => new HttpContextAccessor
            {
                HttpContext = context
            });

            services.AddScoped<IServiceManager, ServiceManager>();

            //configure current user accessor as provider
            var serviceProvider = services.BuildServiceProvider();

            //initialize database with seed data and context accessors services
            var databaseContext = serviceProvider.GetRequiredService<AppDbContext>();
            AppDbInitializer.Initialize(databaseContext);

            // Create the services from configured providers
            Mapper = AutoMapperFactory.Create();
            MachineDateTime = new DateTimeTest();
            TokenService = new TokenServiceTest();
            Context = databaseContext;
            UserManager = serviceProvider.GetRequiredService<UserManager<ZenithUser>>();
            CurrentUserContext = new CurrentUserContextTest(UserManager);
            ServiceMgr = serviceProvider.GetRequiredService<IServiceManager>();

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AppDbContextTestFactory.Destroy(Context);
            }
        }
    }
}
