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
    public void GetContentBlock(string key, string storeId)
    {
        var collection = _database.GetCollection<ContentBlock>(ContentBlock.Collection);

        var filter = Builders<ContentBlock>.Filter.And(
            Builders<Common.Model.ContentBlock>.Filter.Eq(x => x.Key, key),
            Builders<Common.Model.ContentBlock>.Filter.Eq(x => x.StoreId, storeId));
        
        var contentBlock = collection.Find(filter).FirstOrDefault();

        ;
        
        
        
        return;
    }
}