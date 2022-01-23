namespace QuotesAPI.ViewModels;

public class APIExceptionViewModel
{
    public string Message { get; set; } = string.Empty;
    
    public APIExceptionViewModel(string message)
    {
        Message = message;
    }
}