using WebApp.CloudApi.Class;
using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Model;
using WebApp.CloudApi.RequestModel;
using Microsoft.AspNetCore.Mvc;
using WebApp.CloudApi.Helper;
using JustEat.StatsD;

namespace WebApp.CloudApi.Controllers;

/// <summary>
/// Account controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IDbHelper _db;
    private readonly ApplicationInstance _application;
    private readonly ILogger<AccountController> _logger;
    IStatsDPublisher stats;

    public AccountController(
    IDbHelper db,
    ApplicationInstance application, ILogger<AccountController> logger, IStatsDPublisher statsPublisher)
    {
        _db = db;
        this._application = application;
        _logger = logger;
        stats = statsPublisher;
        stats.Increment("webapp");
    }

    [BasicAuthorization]
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        if (this._application.Application != id) {
            return Unauthorized();
        }
        return Ok(_db.GetAccount(id));
    }

    [HttpPost(Name = "account")]
    public async Task<IActionResult> Post([FromBody] AccountRequest account)
    {
        _logger.LogInformation("Create Account called");
        if (ModelState.IsValid)
        {
            try
            {
                var existingAccount = _db.GetAccount(account.Email);
                if (existingAccount.Email == account.Email)
                {
                    return BadRequest("Email already exist");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
             _logger.LogInformation(" Account Created");
            return Created("", await _db.SaveAccount(account));
        }
        else
        {
             _logger.LogInformation("Bad Request");
            return BadRequest();
        }
    }


    [BasicAuthorization]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] AccountRequest model)
    {
        return Created("", await _db.UpdateAccount(id, model));
    }
}