using API.Formatters;
using Application.Contracts;
using Application.Services;
using AspNetCoreRateLimit;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection serviceCollection) => 
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });

    public static void ConfigureIisIntegration(this IServiceCollection serviceCollection) =>
        serviceCollection.Configure<IISOptions>(options => { });

    public static void ConfigureLoggerService(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection serviceCollection) =>
        serviceCollection.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void ConfigureServiceManager(this IServiceCollection serviceCollection) =>
        serviceCollection.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureSqlContext(this IServiceCollection serviceCollection, IConfiguration configuration) =>
        serviceCollection.AddDbContext<AppDbContext>(
            opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
        });
    }

    public static void ConfigureResponseCaching(this IServiceCollection services) => 
        services.AddResponseCaching();

    public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => 
        services.AddHttpCacheHeaders(expirationOpt =>
        {
            expirationOpt.MaxAge = 65;
            expirationOpt.CacheLocation = CacheLocation.Private;
        }, validationOpt =>
        {

            validationOpt.MustRevalidate = true;
        });

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        var rateLimitingRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = 10,
                Period = "5m"
            }
        };

        services.Configure<IpRateLimitOptions>(opts =>
        {
            opts.GeneralRules = rateLimitingRules;
        });
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

}