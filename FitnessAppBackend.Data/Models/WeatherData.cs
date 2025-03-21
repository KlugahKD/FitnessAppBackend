namespace FitnessAppBackend.Data.Models;

public class WeatherData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public bool IsRaining { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
}