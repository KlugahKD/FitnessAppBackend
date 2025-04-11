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
}


/// <summary>
/// DTO for weekly workout statistics.
/// </summary>
public class WeeklyStatsDto
{
    public int CompletedWorkouts { get; set; }
    public string? TotalWorkoutTime { get; set; }
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