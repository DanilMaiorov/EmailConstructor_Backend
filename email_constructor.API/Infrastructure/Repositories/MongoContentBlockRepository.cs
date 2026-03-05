using email_constructor.Domain.Model;
using email_constructor.Infrastructure.Interfaces;
using Google.Protobuf.WellKnownTypes;
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
        
        if (blockTypes.Count > 0)
            filter = filter & f.In(x => x.Key, blockTypes);
        
        var contentBlocks = await collection.Find(filter).ToListAsync();
        
        return contentBlocks;
    }

    public async Task<List<BlockWrapper>> GetContentBlockWrappers()
    {
        var collection = _database.GetCollection<BlockWrapper>(BlockWrapper.Collection);
                    
        var blockWrappers = await collection.Find(Builders<BlockWrapper>.Filter.Empty).ToListAsync();
        
        return blockWrappers;
        
    }
}