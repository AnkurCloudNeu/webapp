using System.Text.Json.Serialization;

namespace WebApp.CloudApi.ResponseModel;
public class AccountResponse
{ 
    [JsonPropertyName("account_id")]
    public Guid AccountId { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    public DateTime AccountCreated { get; set; }
    
    [JsonPropertyName("account_updated")]
    public DateTime AccountUpdated { get; set; }
}