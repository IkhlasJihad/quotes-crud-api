using Microsoft.AspNetCore.Mvc;
using QuotesAPI.Services;
using QuotesAPI.ViewModels;
using QuotesAPI.DTOs;
using QuotesAPI.Exceptions;

namespace QuotesAPI.Controllers;

[ApiController]
[Route("quotes/")]
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
            return Ok(new APIResponseViewModel(result.Item1, result.Item2));    
        }
        catch (PageOutOfRangeException ex)
        {
            return BadRequest(new APIExceptionViewModel(ex.Message));
        }
        catch (Exception ex)
        {
            return NotFound(new APIExceptionViewModel(ex.Message));
        }   
    }

    [HttpGet("random")]
    public async Task<ActionResult> RandomQuote(){
        try
        {
            var result = await _quotesService.RandomQuote();
            return Ok(new APIResponseViewModel(1, result));    
        }
        catch (Exception ex)
        {
            return NotFound(new APIExceptionViewModel(ex.Message));
        }   
    }

    [HttpGet("author")]
    public async Task<ActionResult> QuotesbyAuthor(string? author, int page=1){
        try
        {
            if(string.IsNullOrEmpty(author))
                return BadRequest(new APIExceptionViewModel("author field is required"));
            var result = await _quotesService.QuotesByAuthorAsync(author, page);
            return Ok(new APIResponseViewModel(result.Item1, result.Item2));   
        }
        catch (EmptyResultException)
        {
            return NoContent();
        }
        catch (PageOutOfRangeException ex)
        {
            return BadRequest(new APIExceptionViewModel(ex.Message));
        }
        catch (Exception ex)
        {
            return NotFound(new APIExceptionViewModel(ex.Message));
        }
           
    }
    [HttpGet("book")]
    public async Task<ActionResult> QuotesbyBook(string? book, int page=1){
        try
        {
            if(string.IsNullOrEmpty(book))
                return BadRequest(new APIExceptionViewModel("book field is required"));
            var result = await _quotesService.QuotesByBookAsync(book, page);
            return Ok(new APIResponseViewModel(result.Item1, result.Item2)); 
        }
        catch (EmptyResultException)
        {
            return NoContent();
        }
        catch (PageOutOfRangeException ex)
        {
            return BadRequest(new APIExceptionViewModel(ex.Message));
        }
        catch (Exception ex)
        {
           return NotFound(new APIExceptionViewModel(ex.Message));
        }  
    }

    [HttpGet("tag")]
    public async Task<ActionResult> QuotesbyTag(string? tag,  int page=1){
        try
        {
            if(string.IsNullOrEmpty(tag))
                return BadRequest(new APIExceptionViewModel("tag field is required"));
            var result = await _quotesService.QuotesByTagAsync(tag, page);
            return Ok(new APIResponseViewModel(result.Item1, result.Item2));
        }
        catch (EmptyResultException)
        {
            return NoContent();
        }
        catch (PageOutOfRangeException ex)
        {
            return BadRequest(new APIExceptionViewModel(ex.Message));
        }
        catch (Exception ex)
        {
           return NotFound(new APIExceptionViewModel(ex.Message));
        }         
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddQuote([FromForm]CreateQuote dto){
       try
       {
            var result = await _quotesService.AddQuoteAsync(dto);
            if(result is null)
                return NotFound(new APIExceptionViewModel("Duplicate Text"));
            return Ok(new APIResponseViewModel(1, result)); 
       }
       catch (Exception ex)
       {
            if(ex is MissedAuthorFieldException || ex is MissedTextFieldException)
                return BadRequest(new APIExceptionViewModel(ex.Message));
            return NotFound(new APIExceptionViewModel(ex.Message));
       }     
    }
    [HttpPost("addMany")]
    public async Task<ActionResult> AddMultipleQuotes([FromForm]List<CreateQuote> dto){
        try
        {
            var result = await _quotesService.AddMultipleQuotesAsync(dto);
            return Ok(new APIResponseViewModel(1, result)); 
        }
        catch (Exception ex)
        {
            if(ex is MissedAuthorFieldException || ex is MissedTextFieldException)
                return BadRequest(new APIExceptionViewModel(ex.Message));
            return NotFound(new APIExceptionViewModel(ex.Message));
        }             
    }
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteQuote(string? id){
        try
        {
            if(string.IsNullOrEmpty(id))
                return BadRequest(new APIExceptionViewModel("id is required"));
            await _quotesService.DeleteAsync(id);
            return Ok(new APIResponseViewModel());
        }
        catch (Exception ex)
        {
            return NotFound(new APIExceptionViewModel(ex.Message));
        }
    }
    [HttpPut("update")]
    public async Task<ActionResult> UpdateQuote([FromForm]UpdateQuote dto){
        try
        {
            var result = await _quotesService.UpdateAsync(dto);
            if(result.ModifiedCount == 0)
                return StatusCode(StatusCodes.Status304NotModified);
            else
                return Ok(new APIResponseViewModel());
        }
        catch (Exception ex)
        {
            if(ex is MissedAuthorFieldException || ex is MissedTextFieldException)
                return BadRequest(new APIExceptionViewModel(ex.Message));
            return NotFound(new APIExceptionViewModel(ex.Message));
        }
    }
    [HttpGet("search")]
    public async Task<ActionResult> Search(string? keyword, int page=1){
        try
        {
            if(String.IsNullOrEmpty(keyword))
                return await All();
            var result = await _quotesService.Search(keyword, page);
            return Ok(new APIResponseViewModel(result.Item1, result.Item2));
        }
        catch (EmptyResultException)
        {
            return NoContent();
        }
        catch (PageOutOfRangeException ex)
        {
            return BadRequest(new APIExceptionViewModel(ex.Message));
        }
        catch (Exception ex)
        {
            return NotFound(new APIExceptionViewModel(ex.Message));
        }
        
    }
}