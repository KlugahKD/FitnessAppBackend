namespace FitnessAppBackend.Business.DTO;

public class ExerciseLogRequest
{
    public string ExerciseName { get; set; }
    public int Duration { get; set; }
    public int CaloriesBurned { get; set; }
}