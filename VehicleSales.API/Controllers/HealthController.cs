using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VehicleSales.API.Controllers;

/// <summary>
/// Controller para health checks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class HealthController(HealthCheckService healthCheckService) : ControllerBase
{
    /// <summary>
    /// Endpoint de health check
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await healthCheckService.CheckHealthAsync();
        
        return report.Status == HealthStatus.Healthy 
            ? Ok(new { status = "Healthy", checks = report.Entries })
            : StatusCode(503, new { status = "Unhealthy", checks = report.Entries });
    }

    /// <summary>
    /// Endpoint simplificado para liveness probe
    /// </summary>
    [HttpGet("live")]
    public IActionResult GetLive()
    {
        return Ok(new { status = "alive" });
    }

    /// <summary>
    /// Endpoint para readiness probe
    /// </summary>
    [HttpGet("ready")]
    public async Task<IActionResult> GetReady()
    {
        var report = await healthCheckService.CheckHealthAsync();
        
        return report.Status == HealthStatus.Healthy 
            ? Ok(new { status = "ready" })
            : StatusCode(503, new { status = "not ready" });
    }
}
