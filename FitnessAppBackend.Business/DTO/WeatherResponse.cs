namespace FitnessAppBackend.Business.DTO;

public class WeatherResponse
{
    public List<WeatherInfo> Weather { get; set; } = new();
    public MainInfo Main { get; set; } = new();
    public string Name { get; set; } = string.Empty;
}

public class WeatherInfo
{
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class MainInfo
{
    public double Temp { get; set; }
    public int Humidity { get; set; }
}