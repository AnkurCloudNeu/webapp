using WebApp.CloudApi.Class;
using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Model;
using WebApp.CloudApi.RequestModel;
using Microsoft.AspNetCore.Mvc;
using WebApp.CloudApi.Helper;
using JustEat.StatsD;
using WebApp.CloudApi.DynamoDb;
using WebApp.CloudApi.Interface;

namespace WebApp.CloudApi.Controllers;

/// <summary>
/// Account controller
/// </summary>
[ApiController]
[Route("v1")]
public class VerifyEmailController : ControllerBase
{
    private readonly IDbHelper _db;
    private readonly ApplicationInstance _application;
    private readonly ILogger<AccountController> _logger;
    IStatsDPublisher stats;

    public VerifyEmailController(
    IDbHelper db,
    IUserCreator user,
    ApplicationInstance application, 
    ILogger<AccountController> logger, 
    IStatsDPublisher statsPublisher)
    {
        _db = db;
        this._application = application;
        _logger = logger;
        stats = statsPublisher;
        stats.Increment("webapp");
    }

    [HttpGet("verifyEmail")]
    public IActionResult Get(string email, Guid token)
    {
        _db.VerifyAccount(email);
        return Ok("Account has been VerIfied");
    }
}