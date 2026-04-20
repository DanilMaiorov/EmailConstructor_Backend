using email_constructor.Api.Extensions;
// using email_constructor.Api.Services;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Services;
using email_constructor.Cache;
using email_constructor.Infrastructure.DatabaseInitializer;
using email_constructor.Infrastructure.Interfaces;
using email_constructor.Infrastructure.Repositories;
using email_constructor.Options;

namespace email_constructor.Extensions;

public static class CommonDiExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddAndValidateOptions<ProductCacheOptions>();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IContentBlockRepository, MongoContentBlockRepository>();

        return services;
    }

    public static IServiceCollection AddBlockServices(this IServiceCollection services)
    {
        services.AddScoped<IContentBlockService, ContentBlockService>();

        return services;
    }
    
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
    
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IProductCache, ProductCache>();

        return services;
    }
    
    public static IServiceCollection AddParser(this IServiceCollection services)
    {
        services.AddScoped<IParserService, ExcelParserService>();

        return services;
    }
    
    public static IServiceCollection AddFileUploader(this IServiceCollection services)
    {
        services.AddScoped<IFileUploadService, FileUploadService>();

        return services;
    }

    public static IServiceCollection AddDatabaseIndexes(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseInitializer>();

        return services;
    }
    
    public static void MapEmailConstructorApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // endpoints.MapGrpcService<EmailContentBlockService>();
        endpoints.MapControllers();
    }
    //
    // public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddGrpcClient<EmailContentBlockService.EmailContentBlockServiceClient>(options =>
    //     {
    //         var emailConstructorApiUrl = configuration["GrpcClients:Addresses:EmailConstructor"];
    //         options.Address = new Uri(emailConstructorApiUrl);
    //     });
    //
    //     return services;
    // }
    
    private static IServiceCollection AddAndValidateOptions<T>(this IServiceCollection serviceCollection, string section = default)
        where T : class
    {
        serviceCollection
            .AddOptionsWithValidateOnStart<T>()
            .BindConfiguration(section ?? typeof(T).Name)
            .ValidateDataAnnotations();

        return serviceCollection;
    }
}
