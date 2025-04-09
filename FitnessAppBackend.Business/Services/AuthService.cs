using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FitnessAppBackend.Business.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger)
    : IAuthService
{
    public async Task<ServiceResponse<AuthResponse>> RegisterAsync(RegisterModel model)
    {
        try
        {
            logger.LogInformation("Registering user");
            
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
                PreferredAvatar = model.AvatarChoice
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                logger.LogDebug("Failed to create user");
                
               return ResponseHelper.FailedDependencyResponse<AuthResponse>("Failed to create user");
            }

            var response = new AuthResponse
            {
                Email = user.Email,
                UserId = user.Id,
                FirstName = user.FirstName, 
                LastName = user.LastName
            };
            
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