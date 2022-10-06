using Microsoft.AspNetCore.Authorization;

namespace WebApp.CloudApi.Class;

public class BasicAuthorizationAttribute : AuthorizeAttribute
{
    public BasicAuthorizationAttribute()
    {
        Policy = "BasicAuthentication";
    }
}
