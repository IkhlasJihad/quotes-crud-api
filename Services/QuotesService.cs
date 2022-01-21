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
    private readonly string[] RANDOM_TAGS;
    public QuotesService(IQuotesDBContext _db){
        _quotesCollection = _db.GetQuotesCollection<QuotesDBModel>();
        RANDOM_TAGS = new []{ "حكمة", "أدب", "حياة", "شعر", "inspirational", "motivation", "life", "wisdom", "hope", "think", "poetry"};
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

    public async Task<string> RandomQuote(){
        try
        {
            var filter = Builders<QuotesDBModel>.Filter.AnyIn(q => q.Tags, RANDOM_TAGS);
            FindOptions<QuotesDBModel> findOptions = new FindOptions<QuotesDBModel>(){
                Projection = Builders<QuotesDBModel>.Projection.Include(q => q.Text)
            };         
            var quotes = await (await _quotesCollection.FindAsync(filter, findOptions)).ToListAsync();
            if(quotes.Count > 0){
                var randomIndex = new Random().Next(quotes.Count);
                return quotes[(int)randomIndex].Text;
            }
            else
                throw new EmptyResultException();
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
            var filter = Builders<QuotesDBModel>.Filter.Eq(q => q.Author, author);
            await IsExisted(filter);
            var result = await _quotesCollection.AggregateByPage(
                filter,
                page: page
            ); 
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
            var filter = Builders<QuotesDBModel>.Filter.Eq(q => q.Book, book);
            await IsExisted(filter);
            var result = await _quotesCollection.AggregateByPage(
                filter,
                page: page
            );
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
            var filter =  Builders<QuotesDBModel>.Filter.AnyEq(q => q.Tags, tag);
            await IsExisted(filter);
            var result =  await _quotesCollection.AggregateByPage(
                filter,
                page: page
            ); 
            return result;
        }
        catch (Exception ex)
        {     
            throw ex;
        }
    }
    public async Task<QuotesDBModel?> AddQuoteAsync(CreateQuote dto){
        try
        {
            if(String.IsNullOrEmpty(dto.Text))
                throw new MissedTextFieldException();
            if(String.IsNullOrEmpty(dto.Author))
                throw new MissedAuthorFieldException();
            var quote = await GetQuoteByText(dto.Text);
            var newQuote = new QuotesDBModel();
            if(quote is null){
                newQuote.Text = dto.Text;
                newQuote.Author = dto.Author;
                newQuote.Book = dto.Book;
                newQuote.Tags = dto.Tags;
                await _quotesCollection.InsertOneAsync(newQuote); 
                return newQuote; 
            }
            else return null;
        }
        catch (Exception ex)
        {   
            throw ex;
        }
    }
    public async Task<List<QuotesDBModel>> AddMultipleQuotesAsync(List<CreateQuote> dto){
        try
        {
            List<QuotesDBModel> newQuotes = new List<QuotesDBModel>();
            foreach (var q in dto)
            {
                if(String.IsNullOrEmpty(q.Text))
                    throw new MissedTextFieldException();
                if(String.IsNullOrEmpty(q.Author))
                    throw new MissedAuthorFieldException();
                var quote = await GetQuoteByText(q.Text);
                if(quote is null)
                        newQuotes.Add( new QuotesDBModel(){
                            Text = q.Text,
                            Author = q.Author,
                            Book = q.Book,
                            Tags = q.Tags
                        });
            }
            await _quotesCollection.InsertManyAsync(newQuotes); 
            return newQuotes;   
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
            return result;
        } catch(Exception ex){
            throw ex;
        }
    }

    public async Task IsExisted(FilterDefinition<QuotesDBModel> filter)
    {
        var result =  await _quotesCollection.Find(filter).FirstOrDefaultAsync();
        if(result is null)
            throw new EmptyResultException();
        return;
    }
}