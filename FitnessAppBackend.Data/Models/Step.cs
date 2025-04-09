namespace FitnessAppBackend.Data.Models;

public class Step
{
    public string Id { get; set; }
    public string Description { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsCompleted { get; set; } 
    public string ExerciseId { get; set; }
}