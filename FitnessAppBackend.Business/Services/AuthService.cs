using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitnessAppBackend.Business.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        var userExists = await userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            throw new InvalidOperationException("User already exists!");
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            FitnessGoals = model.FitnessGoals
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Email = user.Email!,
            UserId = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            throw new InvalidOperationException("Invalid password");
        }

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Email = user.Email!,
            UserId = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty
        };
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