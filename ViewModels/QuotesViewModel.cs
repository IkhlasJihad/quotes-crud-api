namespace QuotesAPI.ViewModels;

public class QuotesViewModel{
    public string Text { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Book { get; set; }
    public string[]? Tags { get; set; }
}