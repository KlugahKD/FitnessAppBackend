using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IAuthService
{
    Task<ServiceResponse<AuthResponse>> RegisterAsync(RegisterModel model);
    Task<ServiceResponse<AuthResponse>> LoginAsync(LoginModel model);
    string GenerateJwtToken(ApplicationUser user);
}