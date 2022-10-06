using System.ComponentModel.DataAnnotations;

namespace CloudApi.ResponseModel;
public class AccountResponse
{ 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}