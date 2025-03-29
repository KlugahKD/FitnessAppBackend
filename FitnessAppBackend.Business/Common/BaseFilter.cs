namespace FitnessAppBackend.Business.Common;

public class BaseFilter 
{
    public int PageSize { get; set; } = 10;

    public int PageNumber { get; set; } = 1;
    
    public string? Search { get; set; }

    public DateTime? CreatedAt { get; set; }
}