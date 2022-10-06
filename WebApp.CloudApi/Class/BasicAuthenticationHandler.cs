using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using CloudApi.EfCore;
using CloudApi.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CloudApi.Class;

// BasicAuthenticationHandler.cs
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
     private readonly DbHelper _db;
     private readonly IConfiguration _config;
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        EF_DataContext eF_DataContext,
        IConfiguration config
        )
: base(options, logger, encoder, clock)
    {
         _db = new DbHelper(eF_DataContext,config);
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers.Add("WWW-Authenticate", "Basic");

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
        }

        // Get authorization key
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        var authHeaderRegex = new Regex(@"Basic (.*)");

        if (!authHeaderRegex.IsMatch(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
        }

        var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
        var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
        var authUsername = authSplit[0];
        var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");
        
        if( !_db.GetAccount(authUsername, authPassword)) {
            return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));
        }

        var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, "roundthecode");
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
    }
}