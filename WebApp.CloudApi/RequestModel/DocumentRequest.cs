using System.ComponentModel.DataAnnotations;

namespace WebApp.CloudApi.RequestModel;
public class DocumentRequest
{
    public Guid DocumentID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BucketPath { get; set; } = string.Empty;
    public DateTime DocumentCreated { get; set; }

}