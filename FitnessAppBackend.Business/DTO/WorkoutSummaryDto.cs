using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.DTO;

public class WorkoutSummaryDto
{
    public List<Exercise> PastWorkouts { get; set; } = new();
}