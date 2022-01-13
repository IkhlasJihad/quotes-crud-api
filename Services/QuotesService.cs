using MongoDB.Driver;
using MongoDB.Driver.Linq;
using QuotesAPI.Models;
using QuotesAPI.Extensions;
using QuotesAPI.DTOs;
using QuotesAPI.Data;
using QuotesAPI.Exceptions;

namespace QuotesAPI.Services;

public class QuotesService : IQuotesService
{
    private readonly IMongoCollection<QuotesDBModel> _quotesCollection;
    public QuotesService(IQuotesDBContext _db){
        _quotesCollection = _db.GetQuotesCollection<QuotesDBModel>();
    }

    public async Task<(int, IReadOnlyList<QuotesDBModel>)> QuotesAsync(int page=1){
        try
        {
            return await _quotesCollection.AggregateByPage(
                Builders<QuotesDBModel>.Filter.Empty,
                page: page
            );    
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<QuotesDBModel> GetQuoteById(string id){
        try
        {
            var result = await _quotesCollection.Find(q => q.Id == id).FirstOrDefaultAsync();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<QuotesDBModel> GetQuoteByText(string text){
        try
        {
            var result = await _quotesCollection.Find(q => q.Text == text).FirstOrDefaultAsync();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
        
    public async Task<(int, IReadOnlyList<QuotesDBModel>)> QuotesByAuthorAsync(string author, int page=1){
        try
        {
            var result = await _quotesCollection.AggregateByPage(
                Builders<QuotesDBModel>.Filter.Eq(q => q.Author, author),
                page: page
            ); 
            if(result.Item1 == 0){
                throw new EmptyResultException();
            }
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        } 
    }
    public async Task<(int, IReadOnlyList<QuotesDBModel>)> QuotesByBookAsync(string book, int page=1){ 
        try
        {
            var result = await _quotesCollection.AggregateByPage(
                Builders<QuotesDBModel>.Filter.Eq(q => q.Book, book),
                page: page
            );
            if(result.Item1 == 0){
                throw new EmptyResultException();
            }
            return result;    
        }
        catch (Exception ex)
        {  
            throw ex;
        }       
    }
    public async Task<(int, IReadOnlyList<QuotesDBModel>)> QuotesByTagAsync(string tag, int page=1){
        try
        { 
            var result =  await _quotesCollection.AggregateByPage(
                Builders<QuotesDBModel>.Filter.AnyEq(q => q.Tags, tag),
                page: page
            ); 
            if(result.Item1 == 0){
                throw new EmptyResultException();
            }
            return result;
        }
        catch (Exception ex)
        {     
            throw ex;
        }
    }
    public async Task AddQuoteAsync(CreateQuote dto){
        try
        {
            if(String.IsNullOrEmpty(dto.Text))
                throw new MissedTextFieldException();
            if(String.IsNullOrEmpty(dto.Author))
                throw new MissedAuthorFieldException();
            var quote = await GetQuoteByText(dto.Text);
            if(quote is null)
                await _quotesCollection.InsertOneAsync(new QuotesDBModel(){
                        Text = dto.Text,
                        Author = dto.Author,
                        Book = dto.Book,
                        Tags = dto.Tags
                    });    
        }
        catch (Exception ex)
        {   
            throw ex;
        }
    }
    public async Task AddMultipleQuotesAsync(List<CreateQuote> dto){
        try
        {
            var quotes = new List<QuotesDBModel>();
            foreach (var q in dto)
            {
                if(String.IsNullOrEmpty(q.Text))
                    throw new MissedTextFieldException();
                if(String.IsNullOrEmpty(q.Author))
                    throw new MissedAuthorFieldException();
                var quote = await GetQuoteByText(q.Text);
                if(quote is null)
                        quotes.Add( new QuotesDBModel(){
                            Text = q.Text,
                            Author = q.Author,
                            Book = q.Book,
                            Tags = q.Tags
                        });
            }
            await _quotesCollection.InsertManyAsync(quotes);    
        }
        catch (Exception ex)
        {
            throw ex;
        }    
    }

    public async Task<bool> DeleteAsync(string _id){
        try
        {
            var quote = await GetQuoteById(_id);
            await _quotesCollection.DeleteOneAsync(x => x.Id == _id);
            return true;    
        }
        catch (Exception ex)
        { 
            throw ex;
        }  
    }
    public async Task<ReplaceOneResult> UpdateAsync(UpdateQuote dto){
        try
        {
            if(String.IsNullOrEmpty(dto.Text))
                throw new MissedTextFieldException();
            if(String.IsNullOrEmpty(dto.Author))
                throw new MissedAuthorFieldException();
            var updatedQuote = new QuotesDBModel()
            {
                Id = dto.Id,
                Text = dto.Text,
                Author = dto.Author,
                Book = dto.Book,
                Tags = dto.Tags
            };
            var quote = await GetQuoteByText(dto.Text);
            if(quote is null)
            {
                var result = await _quotesCollection.ReplaceOneAsync(q => q.Id.Equals(dto.Id), 
                updatedQuote, new ReplaceOptions { IsUpsert = true });  
                if(!result.IsAcknowledged)
                    throw new UpdateFailedException();
                if(result.MatchedCount == 0)
                    throw new NoMatchingQuoteException(); 
                return result;
            }
            else
                throw new Exception("The quote already exists!\nUpdates aren't applied");
        }
        catch (Exception ex)
        {   
            throw ex;
        }    
    }

    public async Task<(int, IReadOnlyList<QuotesDBModel>)> Search(string keyword, int page){
        try
        {
            var result =  await _quotesCollection.AggregateByPage(
            /*
            Builders<QuotesDBModel>.Filter.Or(
                new []{
                    Builders<QuotesDBModel>.Filter.AnyEq(q => q.Tags, keyword),
                    Builders<QuotesDBModel>.Filter.Text(keyword, new TextSearchOptions(){
                        Language = lang
                    })
                }
            ),
            */
                Builders<QuotesDBModel>.Filter.Text(keyword), page
            );
            if(result.Item2 is null || result.Item2.Count == 0)
                throw new NoMatchingQuoteException();
            return result;
        } catch(Exception ex){
            throw ex;
        }
    }
}