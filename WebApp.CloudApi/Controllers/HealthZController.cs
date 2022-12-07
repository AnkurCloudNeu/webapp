using WebApp.CloudApi.Class;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using WebApp.CloudApi.Model;
using Amazon.S3.Model;
using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Controllers;

/// <summary>
/// Healthz controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthZController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Good", "Bad"
    };

    private readonly IAmazonS3 _s3Client;
    private readonly IDbHelper _db;
    public HealthZController(ILogger<HealthZController> logger,
    IAmazonS3 s3Client,
    IDbHelper dbHelper)
    {
        logger = logger;
        _db = dbHelper;
        _s3Client = s3Client;
    }

    /// <summary>
    /// Get health status
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "healthz")]
    public IEnumerable<String> Get()
    {
        return Summaries;
    }

    [BasicAuthorization]
    [HttpPost(Name = "document")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, string? prefix)
    {
        string bucketName = DotNetEnv.Env.GetString("BucketName");
        var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        string key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        if (!bucketExists)
            return NotFound($"Bucket {bucketName} does not exist.");
            try {
                var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
                if(s3Object != null) {
                    var split = key.Split(".");
                    key = split[0] + "_copy." + split[1];
                }
            } catch (Exception ex) {
               
            }
        var request = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = file.OpenReadStream()
        };
        request.Metadata.Add("Content-Type", file.ContentType);
        var result = await _s3Client.PutObjectAsync(request);
        var itemUrl = $"https://{bucketName}.s3.us-east-1.amazonaws.com/" + request.Key;
        await _db.SaveDocument(new DocumentRequest
        {
            BucketPath = itemUrl,
            Name = request.Key,
        });
        return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
    }

    [BasicAuthorization]
    [HttpGet("document")]
    public async Task<IActionResult> GetFileByKeyAsync(string key)
    {
        string bucketName = DotNetEnv.Env.GetString("BucketName");
        var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
        var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
        return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
    }

    [BasicAuthorization]
    [HttpDelete("document")]
    public async Task<IActionResult> DeleteFileAsync(string key)
    {
        string bucketName = DotNetEnv.Env.GetString("BucketName");
        var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
        await _s3Client.DeleteObjectAsync(bucketName, key);
        await _db.DeleteDocument(key);
        return NoContent();
    }

    [BasicAuthorization]
    [HttpGet("document/get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
        var request = new ListObjectsV2Request()
        {
            BucketName = bucketName,
            Prefix = prefix
        };
        var result = await _s3Client.ListObjectsV2Async(request);
        var s3Objects = result.S3Objects.Select(s =>
        {
            var urlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = s.Key,
                Expires = DateTime.UtcNow.AddMinutes(1)
            };
            return new S3ObjectDto()
            {
                Name = s.Key.ToString(),
                PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
            };
        });
        return Ok(s3Objects);
    }
}
