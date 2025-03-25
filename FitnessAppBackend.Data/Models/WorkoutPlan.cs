namespace FitnessAppBackend.Data.Models;

public class WorkoutPlan
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    public string Name{ get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}