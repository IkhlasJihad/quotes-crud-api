using System.Collections.Generic;
using System.Threading.Tasks;
using QuotesAPI.Models;
using QuotesAPI.DTOs;
using MongoDB.Driver;

namespace QuotesAPI.Services;
public interface IQuotesService {
    Task<(int totalPages, IReadOnlyList<QuotesDBModel>)> QuotesAsync(int page=1);
    Task<QuotesDBModel> GetQuoteById(string id);
    Task<(int totalPages, IReadOnlyList<QuotesDBModel>)> QuotesByAuthorAsync(string author, int page=1);
    Task<(int totalPages, IReadOnlyList<QuotesDBModel>)> QuotesByBookAsync(string book, int page=1);
    Task<(int totalPages, IReadOnlyList<QuotesDBModel>)> QuotesByTagAsync(string tag, int page=1);
    Task AddQuoteAsync(CreateQuote dto);
    Task AddMultipleQuotesAsync(List<CreateQuote> dto);
    Task<ReplaceOneResult> UpdateAsync(UpdateQuote dto);
    Task<bool> DeleteAsync(string _id);
    Task<(int, IReadOnlyList<QuotesDBModel>)> Search(string keyword, int page);
}