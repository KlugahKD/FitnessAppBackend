using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

/// <summary>
/// Controller for handling dashboard-related actions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    /// Gets the dashboard overview for the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A comprehensive dashboard overview.</returns>
    [HttpGet("overview")]
    public async Task<IActionResult> GetDashboardOverview([FromQuery] string userId)
    {
        var dashboardOverview = await dashboardService.GetDashboardOverviewAsync(userId);
        
        return ActionResultHelper.ToActionResult(dashboardOverview);
    }
}