namespace QuotesAPI.Data;

public class DBSettings{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; }  = string.Empty;
    public string QuotesCollectionName { get; set; }  = string.Empty;
}