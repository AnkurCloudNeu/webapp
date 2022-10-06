namespace WebApp.CloudApi.ResponseModel;
public class AccountResponse
{ 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime AccountCreated { get; set; }
    public DateTime AccountUpdated { get; set; }
}