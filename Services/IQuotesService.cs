using QuotesAPI.Models;
using QuotesAPI.ViewModels;
using QuotesAPI.DTOs;
using MongoDB.Driver;

namespace QuotesAPI.Services;
public interface IQuotesService {
    Task<(int, IReadOnlyList<QuotesViewModel>)> QuotesAsync(int page=1);
    Task<string> RandomQuote();
    Task<(int totalPages, IReadOnlyList<QuotesViewModel>)> QuotesByAuthorAsync(string author, int page=1);
    Task<(int totalPages, IReadOnlyList<QuotesViewModel>)> QuotesByBookAsync(string book, int page=1);
    Task<(int totalPages, IReadOnlyList<QuotesViewModel>)> QuotesByTagAsync(string tag, int page=1);
    Task<QuotesDBModel?> AddQuoteAsync(CreateQuote dto);
    Task<List<QuotesDBModel>> AddMultipleQuotesAsync(List<CreateQuote> dto);
    Task<ReplaceOneResult> UpdateAsync(UpdateQuote dto);
    Task<bool> DeleteAsync(string _id);
    Task<(int, IReadOnlyList<QuotesViewModel>)> Search(string keyword, int page);
}