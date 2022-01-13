using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuotesAPI.Models;

namespace QuotesAPI.Data;

public class QuotesDBContext : IQuotesDBContext {
    private IOptions<DBSettings> _settings;
    private MongoClient _mongoClient {get; set;}
    private IMongoDatabase _db {get; set;}
    public QuotesDBContext(IOptions<DBSettings> dbSettings)
    {
        _settings = dbSettings;
        _mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        _db = _mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
    }
    public IMongoCollection<QuotesDBModel> GetQuotesCollection<QuotesDBModel>()
    {
        return _db.GetCollection<QuotesDBModel>(_settings.Value.QuotesCollectionName);
    }
}