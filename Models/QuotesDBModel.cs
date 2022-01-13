using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuotesAPI.Models;
public class QuotesDBModel{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Required]
    public string Id { get; set; }
    [BsonElement("text")]
    [Required]
    public string Text { get; set; } = string.Empty;
    [BsonElement("author")]
    [Required]
    public string Author { get; set; } = string.Empty;
    [BsonElement("book")]
    [BsonIgnoreIfNull]
    public string? Book { get; set; }
    [BsonElement("tags")]
    [BsonIgnoreIfNull]
    public string[]? Tags { get; set; } 
}