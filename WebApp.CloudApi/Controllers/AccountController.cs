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
    private readonly DbHelper _db;
    private readonly ApplicationInstance _application;
    private readonly ILogger<AccountController> _logger;

    public AccountController(EF_DataContext eF_DataContext, 
    IConfiguration config, 
    ApplicationInstance application, ILogger<AccountController> logger)
    {
        _db = new DbHelper(eF_DataContext, config, application);
        this._application = application;
        _logger = logger;
    }

    [BasicAuthorization]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (this._application.Application != id) {
            return Unauthorized();
        }
        return Ok(await _db.GetAccount(id));
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