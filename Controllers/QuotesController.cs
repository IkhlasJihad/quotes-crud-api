using Microsoft.AspNetCore.Mvc;
using QuotesAPI.Services;
using QuotesAPI.ViewModels;
using QuotesAPI.DTOs;

namespace QuotesAPI.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class QuotesController : Controller {
    private readonly IQuotesService _quotesService;
    public QuotesController(IQuotesService quotesService){
        _quotesService = quotesService;
    }

    [HttpGet("all")]
    public async Task<ActionResult> All(int page=1){
        try
        {
            var result = await _quotesService.QuotesAsync(page);
            return Ok(new APIResponseViewModel(true, result.Item1, result.Item2));    
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }   
    }

    [HttpGet("auhtor/{author}")]
    public async Task<ActionResult> QuotesbyAuthor(string author, int page=1){
        try
        {
            var result = await _quotesService.QuotesByAuthorAsync(author, page);
            return Ok(new APIResponseViewModel(true, result.Item1, result.Item2));   
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
           
    }
    [HttpGet("book/{book}")]
    public async Task<ActionResult> QuotesbyBook(string book, int page=1){
        try
        {
            var result = await _quotesService.QuotesByBookAsync(book, page);
            return Ok(new APIResponseViewModel(true, result.Item1, result.Item2)); 
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }  
    }

    [HttpGet("tag/{tag}")]
    public async Task<ActionResult> QuotesbyTag(string tag,  int page=1){
        try
        {
            var result = await _quotesService.QuotesByTagAsync(tag, page);
            return Ok(new APIResponseViewModel(true, result.Item1, result.Item2));
        }
        catch (Exception ex)
        {
           return NotFound(ex.Message);
        }         
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddQuote([FromForm]CreateQuote dto){
       try
       {
            var result = await _quotesService.AddQuoteAsync(dto);
            if(result is null)
                return NotFound("Duplicate text.");
            return Ok(new APIResponseViewModel(true, 0, result)); 
       }
       catch (Exception ex)
       {
           return NotFound(ex.Message);
       }     
    }
    [HttpPost("addMany")]
    public async Task<ActionResult> AddMultipleQuotes([FromBody]List<CreateQuote> dto){
        try
        {
            var result = await _quotesService.AddMultipleQuotesAsync(dto);
            return Ok(new APIResponseViewModel(true,0, result)); 
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }             
    }
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteQuote(string id){
        try
        {
            await _quotesService.DeleteAsync(id);
            return Ok(new APIResponseViewModel(true));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpPut("update")]
    public async Task<ActionResult> UpdateQuote([FromForm]UpdateQuote dto){
        try
        {
            var result = await _quotesService.UpdateAsync(dto);
            if(result.ModifiedCount == 0)
                return Ok(new APIResponseViewModel(false, message:"no updates applied"));
            else
                return Ok(new APIResponseViewModel(true));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet("search")]
    public async Task<ActionResult> Search(string? keyword, int page=1){
        try
        {
            if(String.IsNullOrEmpty(keyword))
                return await All();
            var result = await _quotesService.Search(keyword, page);
            return Ok(new APIResponseViewModel(true, result.Item1, result.Item2));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
        
    }
}