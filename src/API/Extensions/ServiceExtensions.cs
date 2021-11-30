using Contracts;
using LoggerService;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection serviceCollection) => 
        serviceCollection.AddCors(options => 
        { 
            options.AddPolicy("CorsPolicy", builder => 
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()); 
        });

    public static void ConfigureIisIntegration(this IServiceCollection serviceCollection) =>
        serviceCollection.Configure<IISOptions>(options => { });

    public static void ConfigureLoggerService(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<ILoggerManager, LoggerManager>();
}