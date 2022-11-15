using System.Diagnostics;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;

namespace AspNetCore.Aws.Demo
{
    public class CloudWatchExecutionTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IAmazonCloudWatch _amazonCloudWatch;

        public CloudWatchExecutionTimeMiddleware(RequestDelegate next, ILogger<CloudWatchExecutionTimeMiddleware> logger, IAmazonCloudWatch amazonCloudWatch)
        {
            _next = next;
            _logger = logger;
            _amazonCloudWatch = amazonCloudWatch;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            await _next(context);
            stopWatch.Stop();

            try
            {
                await _amazonCloudWatch.PutMetricDataAsync(new PutMetricDataRequest
                {
                    Namespace = "Demo",
                    MetricData = new List<MetricDatum>
                    {
                        new MetricDatum
                        {
                            MetricName = "AspNetExecutionTime",
                            Value = stopWatch.ElapsedMilliseconds,
                            Unit = StandardUnit.Milliseconds,
                            TimestampUtc = DateTime.UtcNow,
                            Dimensions = new List<Dimension>
                            {
                                new Dimension
                                {
                                    Name = "Method",
                                    Value = context.Request.Method
                                },
                                new Dimension
                                {
                                    Name = "Path",
                                    Value = context.Request.Path
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to send CloudWatch Metric");
            } 
        }
    }
}