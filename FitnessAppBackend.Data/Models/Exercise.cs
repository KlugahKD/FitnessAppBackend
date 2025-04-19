namespace FitnessAppBackend.Data.Models;

public class Exercise
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsStarted { get; set; } 
    public bool IsCompleted { get; set; } 
    public string Description { get; set; }
    public bool IsMissed { get; set; } 
    public string UserId { get; set; }
    public DateTime Date { get; set; }
    public List<Step> Steps { get; set; } 
}