using Microsoft.OpenApi.Models;

namespace Zenith.API.Configuration
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration){
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(configuration["API:Version"], new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = configuration["API:Title"],
                    Version = configuration["API:Version"],
                    Description = configuration["API:Description"],
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = configuration["API:Contact:Name"],
                        Email = configuration["API:Contact:Email"],
                        Url = new Uri(configuration["API:Contact:Url"] ?? string.Empty)
                    }
                });
                
            });


            return services;
        }

        public static IServiceCollection AddSwaggerJWTConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(configuration["API:Version"], new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = configuration["API:Title"],
                    Version = configuration["API:Version"],
                    Description = configuration["API:Description"],
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = configuration["API:Contact:Name"],
                        Email = configuration["API:Contact:Email"],
                        Url = new Uri(configuration["API:Contact:Url"] ?? string.Empty)
                    }
                });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

            });


            return services;
        }
    }
}
