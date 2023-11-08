
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Zenith.Core.Infrastructure.Persistence;

namespace Zenith.IntegrationTests
{
    using static TestingFixture;

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Program>()
                    .AddJsonFile("appsettings-testing.json")                    
                    .Build();

                configurationBuilder.AddConfiguration(integrationConfig);
            });

            builder.ConfigureServices((builder, services) =>
            {               
                services
                    .Remove<AppDbContext>()
                    .AddDbContext<AppDbContext>((sp, options) =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("Data"),
                            builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));                
            });

                    
            
        }
    }
}