using System.ComponentModel.DataAnnotations;

namespace CloudApi.RequestModel;
public class AccountRequest
{ 
    [Required(ErrorMessage = "First Name is required.")]
    public string FirstName { get; set; } = string.Empty;
     [Required(ErrorMessage = "Last Name is required.")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
     [Required(ErrorMessage = "Password is required.")]
     [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}