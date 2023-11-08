using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog.Parsing;
using Zenith.Core.Infrastructure.Identity;
using Zenith.Core.Infrastructure.Services;

namespace Zenith.API.Configuration
{
    public static class JwtAuthenticationConfiguration
    {
        public static IServiceCollection AddJWTAuthConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:SecretKey"]));
            var validateIssuer = Convert.ToBoolean(configuration["JWT:ValidateIssuer"]);
            var validateAudience = Convert.ToBoolean(configuration["JWT:ValidateAudience"]);
            var validateIssuerSigningKey = Convert.ToBoolean(configuration["JWT:ValidateIssuerSigningKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateAudience,
                ValidateIssuerSigningKey = validateIssuerSigningKey,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                RequireExpirationTime = false
            };

            if (tokenValidationParameters.ValidateIssuer)
            {
                tokenValidationParameters.ValidIssuer = configuration["JWT:ValidIssuer"];
            }

            if(tokenValidationParameters.ValidateAudience)
            {
                tokenValidationParameters.ValidAudience = configuration["JWT:ValidAudience"];
            }


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.ClaimsIssuer = configuration["JWT:ValidIssuer"];
                    jwtBearerOptions.RequireHttpsMetadata = false;
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.HttpContext.Request.Headers["Authorization"];
                            if (token.Count > 0 && token[0].StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = token[0].Split(" ")[1];
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

            services.AddTransient<ITokenService>(_ => new TokenService(configuration,new MachineDateTime()));

            return services;
        }
    }
}
