namespace FitnessAppBackend.Data.Models;

public class ExerciseLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser? User { get; set; }
}
