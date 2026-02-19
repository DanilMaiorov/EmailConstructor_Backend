using email_constructor.Common.Model;
using email_constructor.Infrastructure.Interfaces;
using MongoDB.Driver;

namespace email_constructor.Infrastructure.Repositories;

public class MongoContentBlockRepository : IContentBlockRepository
{
    private readonly IMongoDatabase _database;

    public MongoContentBlockRepository(IMongoDatabase database)
    {
        _database = database;
    }
    public async Task<List<ContentBlock>> GetContentBlocks(string storeId, List<string> blockTypes)
    {
        var collection = _database.GetCollection<ContentBlock>(ContentBlock.Collection);
        
        var f = Builders<ContentBlock>.Filter;
        var filter = f.Eq(x => x.StoreId, storeId);
        
        if (blockTypes?.Any() == true)
            filter = filter & f.In(x => x.Key, blockTypes);
        
        var contentBlocks = await collection.Find(filter).ToListAsync();
        
        return contentBlocks;
    }
}