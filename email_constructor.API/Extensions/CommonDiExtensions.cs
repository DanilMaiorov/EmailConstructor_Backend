using email_constructor.Application.Interfaces;
using email_constructor.Application.Services;
using email_constructor.Infrastructure.DatabaseInitializer;
using email_constructor.Infrastructure.Interfaces;
using email_constructor.Infrastructure.Repositories;

namespace email_constructor.Extensions;

public static class CommonDiExtensions
{
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

    public static IServiceCollection AddDatabaseIndexes(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseInitializer>();

        return services;
    }
}
