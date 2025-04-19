namespace FitnessAppBackend.Data.Models;

public class HealthAdvice
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Img { get; set; } 
    public DateTime CreatedAt { get; set; }
}