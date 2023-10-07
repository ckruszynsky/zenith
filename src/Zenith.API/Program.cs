using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Zenith.API.Configuration;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddHealthCheckConfiguration(builder.Configuration);
builder.Services.AddEFConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddMediatRConfiguration();
builder.Services.AddCoreServices();


builder.Services.AddControllerConfiguration();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddSerilog();

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


app.UseAuthorization();

app.MapControllers();

app.Run();
