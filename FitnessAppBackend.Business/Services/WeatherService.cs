using System.Net.Http.Json;
using FitnessAppBackend.Business.DTO;
using Microsoft.Extensions.Configuration;

namespace FitnessAppBackend.Business.Services;

public class WeatherService(HttpClient httpClient, IConfiguration configuration) : IWeatherService
{
    private readonly string _apiKey = configuration["WeatherApi:ApiKey"]!;

    public async Task<SimplifiedWeatherData?> GetCurrentWeatherAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

        var response = await httpClient.GetFromJsonAsync<WeatherResponse>(url);
        if (response is null) return null;

        return new SimplifiedWeatherData
        {
            Temperature = response.Main.Temp,
            IsRaining = response.Weather.Any(w =>
                w.Main.Contains("Rain", StringComparison.OrdinalIgnoreCase) ||
                w.Main.Contains("Drizzle", StringComparison.OrdinalIgnoreCase) ||
                w.Description.Contains("rain", StringComparison.OrdinalIgnoreCase))
        };
    }

}