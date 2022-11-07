using WebApp.CloudApi.Class;
using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Model;
using WebApp.CloudApi.RequestModel;
using Microsoft.AspNetCore.Mvc;
using WebApp.CloudApi.Helper;

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

    public AccountController(
    IDbHelper db,
    ApplicationInstance application, ILogger<AccountController> logger)
    {
        _db = db;
        this._application = application;
        _logger = logger;
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
        string connectionString = $"Host={Environment.GetEnvironmentVariable("Host")};Database={Environment.GetEnvironmentVariable("DatabaseName")};Port={Environment.GetEnvironmentVariable("DatabasePort")};Username={Environment.GetEnvironmentVariable("MasterUsername")};Password={Environment.GetEnvironmentVariable("MasterPassword")};";
        _logger.LogInformation(connectionString);
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
                _logger.Log(LogLevel.Error, eventId:0, ex, ex.Message);
            }
             _logger.LogInformation(" Account Created");
            return Created("", await _db.SaveAccount(account));
        }
        else
        {
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