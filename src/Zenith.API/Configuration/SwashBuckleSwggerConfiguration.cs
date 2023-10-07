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
    }
}
