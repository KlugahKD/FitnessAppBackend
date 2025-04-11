using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;

namespace FitnessAppBackend.Business.Services;

/// <summary>
/// Interface for the dashboard service.
/// </summary>
public interface IDashboardService
{
    Task<ServiceResponse<DashboardOverview>> GetDashboardOverviewAsync(string userId);
}