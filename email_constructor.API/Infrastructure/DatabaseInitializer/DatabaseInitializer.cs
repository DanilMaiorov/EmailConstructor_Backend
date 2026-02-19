using email_constructor.Common.Model;
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
    }
    
    private async Task EnsureContentBlocksIndexesAsync()
    {
        var collection = _database.GetCollection<ContentBlock>(nameof(ContentBlock));

        var indexKeys = Builders<ContentBlock>.IndexKeys
            .Ascending(x => x.StoreId)
            .Ascending(x => x.Key);

        var indexModel = new CreateIndexModel<ContentBlock>(
            indexKeys,
            new CreateIndexOptions
            {
                Unique = true,
                Name = "store_key_unique"
            });

        await collection.Indexes.CreateOneAsync(indexModel);
    }
}