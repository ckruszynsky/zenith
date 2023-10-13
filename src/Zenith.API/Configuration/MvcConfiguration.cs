using System.Text.Json;
using System.Text.Json.Serialization;
using Ardalis.Result.AspNetCore;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Zenith.Core.Features.Users;

namespace Zenith.API.Configuration
{
    public static class ControllerConfiguration
    {
        public static IServiceCollection AddControllerConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddControllers()   
                .AddJsonOptions(options=>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());                    
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;                    
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.Configure<ApiBehaviorOptions>(options=> options.SuppressModelStateInvalidFilter = true);

            return services;
        }
    }
}
