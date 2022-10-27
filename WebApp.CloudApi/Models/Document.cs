using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.CloudApi.Model;
[Table("document")]
public class Document
{
    [Key, Required]
    public Guid DocumentID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BucketPath { get; set; } = string.Empty;
    public DateTime DocumentCreated { get; set; }

     public Guid AccountID { get; set; }
    public Account Account { get; set; }
}