using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.DTO;

public class WorkoutSummaryDto
{
    public List<Exercise> CompletedWorkouts { get; set; } = new();
    public List<Exercise> MissedWorkouts { get; set; } = new();
}