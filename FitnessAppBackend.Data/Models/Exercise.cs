namespace FitnessAppBackend.Data.Models;

public class Exercise
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsStarted { get; set; } 
    public bool IsCompleted { get; set; } 
    public string UserId { get; set; }
    public List<Step> Steps { get; set; } 
}