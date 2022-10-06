using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudApi.Model;
[Table("account")]
public class Account
{
    [Key,Required]
    public int AccountID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime AccountCreated { get; set; }
    public DateTime AccountUpdated { get; set; }   
}