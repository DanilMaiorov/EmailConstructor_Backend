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
    }
    
    private async Task EnsureContentBlocksIndexesAsync()
    {
        var collection = _database.GetCollection<DefaultBlock>(nameof(DefaultBlock));

        var indexKeys = Builders<DefaultBlock>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.Type)
            .Ascending(x => x.Variant)
            .Ascending(x => x.StoreId);

        var indexModel = new CreateIndexModel<DefaultBlock>(
            indexKeys,
            new CreateIndexOptions
            {
                Unique = true,
                Name = "store_type_unique"
            });

        await collection.Indexes.CreateOneAsync(indexModel);
    }
}