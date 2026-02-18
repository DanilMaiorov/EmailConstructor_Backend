using email_constructor.Common.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace email_constructor.Extensions;

public static class MongoDiExtensions
{
    public static IServiceCollection RegisterMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDatabase:ConnectionString"];
        var databaseName = configuration["MongoDatabase:Database"];
        
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(databaseName);

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton<IMongoDatabase>(database);

        ConfigureMongo();
        // RegisterEntitiesClassMap();

        return services;
    }

    private static void ConfigureMongo()
    {
        if (BsonSerializer.LookupSerializer<Guid>() is null)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register("EnumStringConvention", pack, t => true);
    }
    
    // private static void RegisterEntitiesClassMap()
    // {
    //     if (!BsonClassMap.IsClassMapRegistered(typeof(ButtonBlock))) 
    //     {
    //         BsonClassMap.RegisterClassMap<ContentBlock>(cm =>
    //         {
    //             cm.AutoMap();
    //             cm.SetIsRootClass(true);
    //             cm.AddKnownType(typeof(ButtonBlock));
    //         });
    //     }
    // }
}