using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterModel model);
    Task<AuthResponse> LoginAsync(LoginModel model);
    string GenerateJwtToken(ApplicationUser user);
}