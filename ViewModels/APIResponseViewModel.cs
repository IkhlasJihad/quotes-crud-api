namespace QuotesAPI.ViewModels;

public class APIResponseViewModel
{
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public int totalPages {get; set;} = 0;
    
    public APIResponseViewModel(int pages=0, object data = null, string message = "done")
    {
        Message = message;
        totalPages = pages;
        Data = data;
    }
}