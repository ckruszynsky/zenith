using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Respawn.Graph;
using Zenith.Common.Domain;
using Zenith.Core.Infrastructure.Persistence;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests
{
    [SetUpFixture]
    public partial class TestingFixture
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static IConfiguration _configuration = null!;
        private static IServiceScopeFactory _scopeFactory = null!;
        private static Respawn.Respawner _respawner = null!;
        private static string _connection = null!;
               
        public const string ArticlesEndpoint = "/api/articles/";
        public const string RegisterEndpoint = "/api/users/";
        public const string UpdateUserEndpoint = "/api/users";
        public const string LoginEndpoint = "/api/users/login";
        public const string ProfileEndpoint = "/api/users/profile";
        public const string TagsEndpoint = "/api/tags/";
        private static HttpClient _httpClient = null;


        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            _factory = new CustomWebApplicationFactory();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
            _connection = _configuration.GetConnectionString("Data") ?? throw new ConfigurationErrorsException("Default Connection could not be located");
            _httpClient = _factory.CreateClient();
            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {               
                TablesToIgnore = new Table[] {
                  "__EFMigrationsHistory"
               },
                DbAdapter = DbAdapter.SqlServer
            });                     
        }

        public static async Task ResetState()
        {
            await _respawner.ResetAsync(_connection);            
        }

        public static HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            return await mediator.Send(request);
        }
             
        public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();

        }
        public static async Task<int> AddAsyncBaseEntity<TEntity>(TEntity entity)
            where TEntity : BaseAuditableEntity
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
            return entity.Id;
        }

        public static async Task<int> CountAsync<TEntity>() where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Set<TEntity>().CountAsync();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }



    }
}
