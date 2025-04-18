using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FitnessAppBackend.Business.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger, IWorkoutService workoutService, ApplicationDbContext context, IServiceScopeFactory serviceScopeFactory)
    : IAuthService
{
  public async Task<ServiceResponse<AuthResponse>> RegisterAsync(RegisterModel model)
{
    try
    {
        logger.LogInformation("Registering user");
        
        // Early validations
        if (string.IsNullOrWhiteSpace(model.FitnessGoals))
        {
            logger.LogDebug("FitnessGoals is null or empty");
            return ResponseHelper.BadRequestResponse<AuthResponse>("Fitness goals are required");
        }
        
        if (!Enum.TryParse<WorkoutPlanType>(model.FitnessGoals, true, out var planType))
        {
            logger.LogDebug("Invalid fitness goal: {FitnessGoals}", model.FitnessGoals);
            return ResponseHelper.BadRequestResponse<AuthResponse>("Invalid fitness goal");
        }
        
        var userExists = await userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            logger.LogDebug("User already exists");
            return ResponseHelper.BadRequestResponse<AuthResponse>("User already exists");
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            FitnessGoals = model.FitnessGoals,
            HowOftenWorkOut = model.HowOftenWorkOut,
            PreferredAvatar = model.AvatarChoice,
            CreatedAt = DateTime.Now
        };
       
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            logger.LogDebug("Failed to create user");
            return ResponseHelper.FailedDependencyResponse<AuthResponse>("Failed to create user or password is weak");
        }
        
        var response = new AuthResponse
        {
            Email = user.Email,
            UserId = user.Id,
            FirstName = user.FirstName, 
            LastName = user.LastName
        };
        
        // Store the data needed for the background task
        string userId = user.Id;
        
        // Use the service provider to create a new scope
        _ = Task.Run(async () => 
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var scopedWorkoutService = scope.ServiceProvider.GetRequiredService<IWorkoutService>();
                    
                    var planRequest = new WorkoutPlanRequest
                    {
                        UserId = userId,
                        PlanType = planType
                    };
                    
                    await scopedWorkoutService.CreateWorkoutPlanAsync(planRequest);
                    logger.LogInformation("Workout plan created successfully for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create workout plan for user {UserId}", userId);
            }
        });
        
        return ResponseHelper.OkResponse(response);
    }
    catch (Exception e)
    {
        logger.LogError(e, "Error registering user");
        return ResponseHelper.InternalServerErrorResponse<AuthResponse>("Something went wrong");
    }
}

    public async Task<ServiceResponse<AuthResponse>> LoginAsync(LoginModel model)
    {
        try
        {
            logger.LogInformation("Logging in user");
            
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                logger.LogDebug("User not found");
                
                return ResponseHelper.BadRequestResponse<AuthResponse>("Invalid login credentials");
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
               logger.LogDebug("Invalid password");
               
                return ResponseHelper.BadRequestResponse<AuthResponse>("Invalid login credentials");
            }

            var response = new AuthResponse
            {
                Token = GenerateJwtToken(user),
                Email = user.Email ?? string.Empty,
                UserId = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty
            };
            
            return ResponseHelper.OkResponse(response);
        }
        catch (Exception e)
        {
           logger.LogError(e, "Error logging in user");
           
            return ResponseHelper.InternalServerErrorResponse<AuthResponse>("Something went wrong");
        }
    }

    public string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}