using FitnessAppBackend.Business.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Helper;

public static class ActionResultHelper
{
    public static IActionResult ToActionResult<T>(ServiceResponse<T> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = response.Code
        };
    }
}