using System.Security.Principal;

namespace WebApp.CloudApi.Class;
public class AuthenticatedUser : IIdentity
{
    public AuthenticatedUser(string authenticationType, bool isAuthenticated, string name)
    {
        AuthenticationType = authenticationType;
        IsAuthenticated = isAuthenticated;
        Name = name;
    }
 
    public string AuthenticationType { get; }
 
    public bool IsAuthenticated { get;}
 
    public string Name { get; }
}