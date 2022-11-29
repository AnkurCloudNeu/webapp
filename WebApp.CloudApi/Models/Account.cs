using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.CloudApi.Model;
[Table("account")]
public class Account
{
    [Key,Required]
    public Guid AccountID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime AccountCreated { get; set; }
    public DateTime AccountUpdated { get; set; }   
    public List<Document> Documents { get; set; }
    public bool Verified  { get; set; }
}