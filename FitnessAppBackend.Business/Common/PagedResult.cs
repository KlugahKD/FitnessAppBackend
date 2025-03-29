namespace FitnessAppBackend.Business.Common;

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    public List<T> Payload { get; set; }
}   