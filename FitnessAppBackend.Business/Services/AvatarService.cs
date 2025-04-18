using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FitnessAppBackend.Business.Services;

public class AvatarService : IAvatarService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AvatarService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _aiApiKey;
    private readonly string _aiEndpoint;

    public AvatarService(
        ApplicationDbContext context, 
        ILogger<AvatarService> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
        _aiApiKey = configuration["AI:ApiKey"];
        _aiEndpoint = configuration["AI:Endpoint"];
    }

    public async Task<ServiceResponse<string>> GetResponseAsync(string userId, string question)
    {
        try
        {
            _logger.LogInformation("User with Id {Id} is interacting with avatar", userId);
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null || string.IsNullOrEmpty(user.PreferredAvatar))
            {
                _logger.LogDebug("User does not have an avatar");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            var avatar = await _context.Avatars.FirstOrDefaultAsync(a => a.Name == user.PreferredAvatar);
            if (avatar == null)
            {
                _logger.LogDebug("Avatar not found");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            if (avatar.Responses.TryGetValue(question, out var predefinedResponse))
            {
                _logger.LogDebug("Found predefined response for question: {Question}", question);
                return ResponseHelper.OkResponse(predefinedResponse);
            }

            var aiResponse = await GenerateAiResponseAsync(avatar, question);
            return ResponseHelper.OkResponse(aiResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting response for user with Id {Id}", userId);
            return ResponseHelper.InternalServerErrorResponse<string>(
                "An error occurred while processing your request.");
        }
    }

    public async Task<ServiceResponse<string>> GetMotivationalMessageAsync(string userId)
    {
        try
        {
            _logger.LogInformation("Fetching motivational message for user with Id {Id}", userId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
            if (string.IsNullOrEmpty(user?.PreferredAvatar))
            {
                _logger.LogDebug("User does not have an avatar");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            var avatar = await _context.Avatars.FirstOrDefaultAsync(a => a.Name == user.PreferredAvatar);
            if (avatar == null)
            {
                _logger.LogDebug("Avatar not found");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            // For motivational messages, we can either use the pre-defined ones or generate a custom one
            if (avatar.MotivationalMessages.Count > 0)
            {
                var random = new Random();
                var message = avatar.MotivationalMessages[random.Next(avatar.MotivationalMessages.Count)];
                return ResponseHelper.OkResponse(message);
            }
            else
            {
                // Generate a custom motivational message based on avatar
                var customMessage = await GenerateMotivationalMessageAsync(avatar);
                return ResponseHelper.OkResponse(customMessage);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting motivational message for user with Id {Id}", userId);
            return ResponseHelper.InternalServerErrorResponse<string>(
                "An error occurred while processing your request.");
        }
    }

    private async Task<string> GenerateAiResponseAsync(Avatar avatar, string question)
    {
        try
        {
            _logger.LogInformation("Generating AI response using Cohere for avatar {AvatarName}", avatar.Name);

            var requestBody = new
            {
                model = "command-r-plus", 
                prompt = $"You are {avatar.Name}, a fitness assistant who specializes in {avatar.Specialization}. " +
                         $"Your description: {avatar.Description}. Answer this fitness-related question: {question}",
                max_tokens = 300,
                temperature = 0.7
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _aiApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PostAsync(_aiEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Cohere API failed with status code {StatusCode}", response.StatusCode);
                return "Hmm, I couldn’t come up with a good answer. Please try again.";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            var generatedText = doc.RootElement.GetProperty("generations")[0].GetProperty("text").GetString();

            return generatedText?.Trim() ?? "Sorry, I couldn’t generate a response this time.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI response");
            return "I'm having trouble answering right now. Try again later!";
        }
    }


    private async Task<string> GenerateMotivationalMessageAsync(Avatar avatar)
    {
        try
        {
            _logger.LogInformation("Generating motivational message for avatar {AvatarName}", avatar.Name);
            
            var prompt = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                {
                    new 
                    {
                        role = "system",
                        content = $"You are {avatar.Name}, a fitness coach specializing in {avatar.Specialization}. " +
                                 $"Generate a short, powerful motivational message (max 15 words) that would inspire " +
                                 $"someone interested in {avatar.Specialization}. " +
                                 $"Make it energetic and empowering. " +
                                 $"Here are examples of your style: " + string.Join(" | ", avatar.MotivationalMessages.Take(3))
                    },
                    new
                    {
                        role = "user",
                        content = "Give me a motivational message for today's workout."
                    }
                },
                max_tokens = 50
            };

            var content = new StringContent(
                JsonSerializer.Serialize(prompt),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _aiApiKey);

            var response = await _httpClient.PostAsync(_aiEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonSerializer.Deserialize<AiCompletionResponse>(responseBody);

            return aiResponse?.Choices[0].Message.Content ?? 
                   "Today is your day to be stronger than your excuses!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating motivational message: {Message}", ex.Message);
            return "Push past your limits today!";
        }
    }

    // Class to deserialize AI API response
    private class AiCompletionResponse
    {
        public Choice[] Choices { get; set; }

        public class Choice
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            public string Content { get; set; }
        }
    }
}