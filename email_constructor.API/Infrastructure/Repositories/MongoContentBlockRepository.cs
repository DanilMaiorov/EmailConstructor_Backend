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
    public async Task<List<DefaultBlock>> GetDefaultBlocks(string storeId, List<string> blockTypes)
    {
        var collection = _database.GetCollection<DefaultBlock>(DefaultBlock.Collection);
        var filter = Builders<DefaultBlock>.Filter.And(
            Builders<DefaultBlock>.Filter.Eq(x => x.StoreId, storeId),
            Builders<DefaultBlock>.Filter.In(x => x.Type, blockTypes));
        
        var contentBlocks = await collection.Find(filter).ToListAsync();
        
        return contentBlocks;
    }

    public async Task<List<DefaultBlockData>> GetDefaultBlocksData(string storeId, string languageId, List<string> blockTypes)
    {
        var collection = _database.GetCollection<DefaultBlockData>(DefaultBlockData.Collection);
        var filter = Builders<DefaultBlockData>.Filter.And(
            Builders<DefaultBlockData>.Filter.Eq(x => x.StoreId, storeId),
            Builders<DefaultBlockData>.Filter.Eq(x => x.LanguageId, languageId),
            Builders<DefaultBlockData>.Filter.In(x => x.Type, blockTypes));
        
        var defaultBlocksData = await collection.Find(filter).ToListAsync();
        
        return defaultBlocksData;
    }

    public async Task<List<BlockWrapper>> GetBlockWrappers()
    {
        var collection = _database.GetCollection<BlockWrapper>(BlockWrapper.Collection);
        var blockWrappers = await collection.Find(Builders<BlockWrapper>.Filter.Empty).ToListAsync();
        
        return blockWrappers;
    }

    // private FilterDefinition<T> BuildFilter<T>(string storeId, List<string> blockTypes) 
    //     where T : IBlockFilter
    // {
    //     var f = Builders<T>.Filter;
    //     var filter = f.Eq(x => x.StoreId, storeId);
    //     
    //     if (blockTypes.Count > 0)
    //         filter = filter & f.In(x => x.Key, blockTypes);
    //     
    //     return filter;
    // }
}