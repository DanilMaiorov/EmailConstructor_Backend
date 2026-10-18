using email_constructor.Domain.Model;
using MongoDB.Driver;

namespace email_constructor.Infrastructure.DatabaseInitializer;

public class DatabaseInitializer
{
    private readonly IMongoDatabase _database;

    public DatabaseInitializer(IMongoDatabase database)
    {
        _database = database;
    }
    
    public async Task InitializeAsync()
    {
        await EnsureContentBlocksIndexesAsync();
        await EnsureContentBlocksDataIndexesAsync();
    }
    
    private async Task EnsureContentBlocksIndexesAsync()
    {
        var collection = _database.GetCollection<DefaultBlock>(
            DefaultBlock.Collection);

        var indexKeys = Builders<DefaultBlock>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.Type)
            .Ascending(x => x.Variant)
            .Ascending(x => x.StoreId);
        
        var indexModel = CreateIndexModel(indexKeys, "store_type_unique");

        await collection.Indexes.CreateOneAsync(indexModel);
    }
    
    private async Task EnsureContentBlocksDataIndexesAsync()
    {
        var collection = _database.GetCollection<DefaultBlockData>(
            DefaultBlockData.Collection);

        var indexKeys = Builders<DefaultBlockData>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.Type)
            .Ascending(x => x.Variant)
            .Ascending(x => x.LanguageId)
            .Ascending(x => x.StoreId);
        
        var indexModel = CreateIndexModel(indexKeys, "default_data_unique");

        await collection.Indexes.CreateOneAsync(indexModel);
    }
    
    private static CreateIndexModel<T> CreateIndexModel<T>(
        IndexKeysDefinition<T> indexKeys,
        string indexName)
        where T : class
    {
        return new CreateIndexModel<T>(
            indexKeys,
            new CreateIndexOptions
            {
                Unique = true,
                Name = indexName
            });
    }
}