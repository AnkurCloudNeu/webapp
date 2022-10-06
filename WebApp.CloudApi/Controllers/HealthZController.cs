using CloudApi.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudApi.Controllers;

/// <summary>
/// Healthz controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthZController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Good", "Bad"
    };

    private readonly ILogger<HealthZController> _logger;

    public HealthZController(ILogger<HealthZController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get health status
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "healthz")]
    public IEnumerable<String> Get()
    {
        return Summaries;
    }
}
