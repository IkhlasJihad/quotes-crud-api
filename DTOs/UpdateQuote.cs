using System.ComponentModel.DataAnnotations;

namespace QuotesAPI.DTOs;
public class UpdateQuote{
    [Required]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Text { get; set; } = string.Empty;
    [Required]
    public string Author { get; set; } = string.Empty;
    public string? Book { get; set; }
    public string[]? Tags { get; set; } 
}