namespace FitnessAppBackend.Business.DTO;

public class DashboardOverview
{
    public double Temperature { get; set; }
    public int TotalWorkouts { get; set; }
    public int Streak { get; set; }
    public WeeklyStatsDto WeeklyStats { get; set; }
    public int DaysWorkedOut { get; set; }
    public string? MotivationalMessage { get; set; }
    public GraphDataDto GraphData { get; set; }
    public string AvgWorkoutTime { get; set; } // e.g., "38 min"
    public string WorkoutTimeDifference { get; set; } // e.g., "-2 min from last week"
    public string GoalCompletionPercentage { get; set; } // e.g., "83%"
    public string GoalCompletionDetails { get; set; } // e.g., "5 of 6 days"
}


/// <summary>
/// DTO for weekly workout statistics.
/// </summary>
public class WeeklyStatsDto
{
    public int CompletedWorkouts { get; set; }
    public int TotalWorkoutTime { get; set; }
    public  int TotalWorkoutsForTheWeek { get; set; }
    public  int DaysWorkedOut { get; set; }
}

/// <summary>
/// DTO for graph data.
/// </summary>
    public class GraphDataDto
    {
        public List<string> X { get; set; }
        public List<int> Y { get; set; }
    }