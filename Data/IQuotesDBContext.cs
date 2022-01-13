using MongoDB.Driver;
using QuotesAPI.Models;

namespace QuotesAPI.Data;

public interface IQuotesDBContext{
    IMongoCollection<QuotesDBModel> GetQuotesCollection<QuotesDBModel>();
}