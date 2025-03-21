using System.Text.Json;
using FitnessAppBackend.Data.Models;
using Microsoft.Extensions.Configuration;

namespace FitnessAppBackend.Business.Services;

public class WeatherService(HttpClient httpClient, IConfiguration configuration) : IWeatherService
{
    public async Task<WeatherData> GetCurrentWeatherAsync(string location)
    {
        var apiKey = configuration["WeatherAPI:Key"];
        var response = await httpClient.GetAsync($"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={location}");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<WeatherData>(content);
        return weatherData ?? new WeatherData();
    }

    public async Task<IEnumerable<string>> GetWorkoutSuggestionsAsync(WeatherData weatherData)
    {
        var suggestions = new List<string>();

        if (weatherData.IsRaining)
        {
            suggestions.Add("Indoor cardio workout");
            suggestions.Add("Home strength training");
            suggestions.Add("Yoga session");
        }
        else if (weatherData.Temperature > 30) // Hot weather
        {
            suggestions.Add("Swimming");
            suggestions.Add("Early morning run");
            suggestions.Add("Indoor gym session");
        }
        else if (weatherData.Temperature < 10) // Cold weather
        {
            suggestions.Add("Indoor HIIT workout");
            suggestions.Add("Gym strength training");
            suggestions.Add("Indoor cycling");
        }
        else // Moderate weather
        {
            suggestions.Add("Outdoor running");
            suggestions.Add("Park workout");
            suggestions.Add("Cycling");
        }

        return suggestions;
    }
}