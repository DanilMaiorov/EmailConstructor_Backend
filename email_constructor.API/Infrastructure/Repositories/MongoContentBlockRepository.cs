using email_constructor.Application.Models;
using email_constructor.Domain.Model;
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
    public async Task<List<DefaultBlock>> GetBlocks(string storeId, List<BlockData> blockTypes)
    {
        var collection = _database.GetCollection<DefaultBlock>(DefaultBlock.Collection);
        
        var blockTypeFilter = blockTypes.Select(b => 
            Builders<DefaultBlock>.Filter.And(
                Builders<DefaultBlock>.Filter.Eq(x => x.Key, b.Key),
                Builders<DefaultBlock>.Filter.Eq(x => x.Type, b.Type),
                Builders<DefaultBlock>.Filter.Eq(x => x.Variant, b.Variant)
            )
        ).ToList();
    
        var filter = Builders<DefaultBlock>.Filter.And(
            Builders<DefaultBlock>.Filter.Eq(x => x.StoreId, storeId),
            Builders<DefaultBlock>.Filter.Or(blockTypeFilter)
        );
        
        return await collection.Find(filter).ToListAsync();
    }

    public async Task<List<DefaultBlockData>> GetBlocksDefaultData(string storeId, string languageId, List<BlockData> blockTypes)
    {
        var collection = _database.GetCollection<DefaultBlockData>(DefaultBlockData.Collection);
        
        var blockTypeFilter = blockTypes.Select(b => 
            Builders<DefaultBlockData>.Filter.And(
                Builders<DefaultBlockData>.Filter.Eq(x => x.Key, b.Key),
                Builders<DefaultBlockData>.Filter.Eq(x => x.Type, b.Type),
                Builders<DefaultBlockData>.Filter.Eq(x => x.Variant, b.Variant)
            )
        ).ToList();
        
        var filter = Builders<DefaultBlockData>.Filter.And(
            Builders<DefaultBlockData>.Filter.Eq(x => x.StoreId, storeId),
            Builders<DefaultBlockData>.Filter.Eq(x => x.LanguageId, languageId),
            Builders<DefaultBlockData>.Filter.Or(blockTypeFilter)
            );
        
        var defaultBlocksData = await collection.Find(filter).ToListAsync();
        
        return defaultBlocksData;
    }

    public async Task<List<BlockWrapper>> GetBlockWrappers()
    {
        var collection = _database.GetCollection<BlockWrapper>(BlockWrapper.Collection);
        var blockWrappers = await collection.Find(Builders<BlockWrapper>.Filter.Empty).ToListAsync();
        
        return blockWrappers;
    }

    // private List<FilterDefinition<T>> BuildBlockTypeFilter<T>(List<BlockData> blockTypes) 
    //     where T : DefaultBlockData, BlockData
    // {
    //     return blockTypes.Select(b => 
    //         Builders<T>.Filter.And(
    //             Builders<T>.Filter.Eq(x => x.Key, b.Key),
    //             Builders<T>.Filter.Eq(x => x.Type, b.Type),
    //             Builders<T>.Filter.Eq(x => x.Variant, b.Variant)
    //         )
    //     ).ToList();
    // }
}