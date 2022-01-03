using System.Text;
using API.Formatters;
using Application.Contracts;
using Application.Services;
using AspNetCoreRateLimit;
using Domain.ConfigurationModels;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
        // var jwtSettings = configuration.GetSection("JwtSettings");
        // var secretKey = Environment.GetEnvironmentVariable("JWTSECRET");
        // var secretKey = jwtSettings["secret"];
        var secretKey = jwtConfiguration.Secret;

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret))
            };
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Prunedge Web API",
                Version = "v1",
                Description = "Prunedge Web API Template",
                TermsOfService = new Uri("https://prunedge.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Daniel Ale",
                    Email = "developer@prunedge.com",
                    Url = new Uri("https://prunedge.com/danielale")
                },
                License = new OpenApiLicense
                {
                    Name = "Prunedge API LICX",
                    Url = new Uri("https://prunedge.com/developer-licence")
                }
            });
            //s.SwaggerDoc("v2", new OpenApiInfo { Title = "Prunedge Web API2", Version = "v2" });

            var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer"
                    },
                    new List<string>()
                }
            });
        });
    }

}