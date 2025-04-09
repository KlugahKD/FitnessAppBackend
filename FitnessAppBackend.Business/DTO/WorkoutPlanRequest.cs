namespace FitnessAppBackend.Business.DTO;

public class WorkoutPlanRequest
{
    public string UserId { get; set; }
    
    public WorkoutPlanType PlanType { get; set; } 
}

public enum WorkoutPlanType
{
    LoseWeight,
    GainMuscle,
    ImproveEndurance,
    IncreaseFlexibility
}