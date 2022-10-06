using Microsoft.AspNetCore.Authorization;

namespace CloudApi.Class;

public class BasicAuthorizationAttribute : AuthorizeAttribute
{
    public BasicAuthorizationAttribute()
    {
        Policy = "BasicAuthentication";
    }
}
