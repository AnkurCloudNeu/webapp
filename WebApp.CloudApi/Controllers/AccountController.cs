using WebApp.CloudApi.Class;
using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Model;
using WebApp.CloudApi.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.CloudApi.Controllers;

/// <summary>
/// Account controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly DbHelper _db;

    public AccountController(EF_DataContext eF_DataContext, IConfiguration config)
    {
        _db = new DbHelper(eF_DataContext, config);
    }

    [BasicAuthorization]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _db.GetAccount(id));
    }

    [HttpPost(Name = "account")]
    public async Task<IActionResult> Post([FromBody] AccountRequest account)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingAccount = _db.GetAccount(account.Email);
                if (existingAccount != null)
                {
                    return BadRequest("Email already exist");
                }
            }
            catch (Exception e)
            {
            }
            return Created("", await _db.SaveAccount(account));
        }
        else
        {
            return BadRequest();
        }
    }


    [BasicAuthorization]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] AccountRequest model)
    {
        return Created("", await _db.UpdateAccount(id, model));
    }
}