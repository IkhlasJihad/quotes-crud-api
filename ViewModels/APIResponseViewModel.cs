namespace QuotesAPI.ViewModels;

public class APIResponseViewModel
{
    public bool Status { get; set; }  = true;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public int totalPages {get; set;} = 0;
    
    public APIResponseViewModel(bool status, int pages=0, object data = null, string message = "done")
    {
        Status = status;
        Message = message;
        totalPages = pages;
        Data = data;
    }
}