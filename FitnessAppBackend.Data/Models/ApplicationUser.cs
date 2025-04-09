using Microsoft.AspNetCore.Identity;

namespace FitnessAppBackend.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? FitnessGoals { get; set; }
    public string? PreferredAvatar { get; set; }
    public string? HowOftenWorkOut { get; set; }
    public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
}