using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Zenith.API.Configuration;
using Zenith.API.Middleware;
using Zenith.Core.Domain.Entities;
using Zenith.Core.Features.Users;
using Zenith.Core.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddCoreServices();
builder.Services.AddHealthCheckConfiguration(builder.Configuration);
builder.Services.AddEFConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddJWTAuthConfiguration(builder.Configuration);
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddMediatRConfiguration();
builder.Services.AddControllerConfiguration();
builder.Services.AddSwaggerJWTConfiguration(builder.Configuration);
builder.Services.AddSerilog();

builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUser.Validator>());

builder.Services.AddTransient<UserManager<ZenithUser>>();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.DeveloperMode = builder.Environment.IsDevelopment();

});


builder.Host.UseSerilog();
var app = builder.Build();



Log.Logger = new LoggerConfiguration()
    .WriteTo.ApplicationInsights(app.Services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces)
    .CreateLogger();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

app.UseSwaggerUI(options =>
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        $"Zenith API version {builder.Configuration["API:Version"]}"));

var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<AppDbInitializer>();

await initializer.InitialiseAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();

public partial class Program { }