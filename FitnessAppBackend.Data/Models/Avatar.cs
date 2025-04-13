using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAppBackend.Data.Models;

public class Avatar
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public List<string> MotivationalMessages { get; set; } = new();
    
    [NotMapped]
    public Dictionary<string, string> Responses { get; set; } = new();
}