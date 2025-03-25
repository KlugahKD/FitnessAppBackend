namespace FitnessAppBackend.Business.Common;

public class ServiceResponse<T>
{
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public List<string>? Errors { get; set; }
    public T? Data { get; set; }
}