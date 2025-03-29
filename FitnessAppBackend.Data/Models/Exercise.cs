namespace FitnessAppBackend.Data.Models;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool RequiresEquipment { get; set; }
    public bool IsIndoor { get; set; }
    public string UserId { get; set; }
}