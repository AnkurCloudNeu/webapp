using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using WebApp.CloudApi.Interface;
using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.DynamoDb;

public class UserCreator : IUserCreator
{
    private readonly IAmazonDynamoDB dynamoDB;
    private readonly IAmazonSQS sqs;
    private readonly IAmazonSimpleNotificationService sns;
    private readonly ILogger<UserCreator> _logger;
    public UserCreator(IAmazonDynamoDB dynamoDB, IAmazonSQS sqs, IAmazonSimpleNotificationService sns, ILogger<UserCreator> logger)
    {
        this.dynamoDB = dynamoDB;
        this.sqs = sqs;
        this.sns = sns;
        _logger = logger;
    }   
    public async Task<bool> CreateUser(AccountRequest user)
    {
        try
        {
             _logger.LogInformation("Instruction 1");
            Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + 300;
            var request = new PutItemRequest
            {
                TableName = "csye6225",
                Item = new Dictionary<string, AttributeValue> {
                {"id", new AttributeValue(user.Email) },
                {"token", new AttributeValue(Guid.NewGuid().ToString()) },
                {"expiry", new AttributeValue(unixTimestamp.ToString()) },
            }
            };
             _logger.LogInformation("Instruction 2");
            var response = await dynamoDB.PutItemAsync(request);
             _logger.LogInformation("Instruction 3");
            var result = response.HttpStatusCode == System.Net.HttpStatusCode.OK;
             _logger.LogInformation("Instruction 4");

            if (result)
            {
                //var client = new AmazonSimpleNotificationServiceClient(region: Amazon.RegionEndpoint.USEast1);
                var requestSns = new PublishRequest
                {
                    Message = $"{user.Email}",
                    TopicArn = DotNetEnv.Env.GetString("SNSName")
                };
                var responseSns = await sns.PublishAsync(requestSns);
             _logger.LogInformation("Instruction 5");


            }
            return result;
        }
        catch (Exception ex)
        {
             _logger.LogInformation(ex.Message);
            return false;
        }
    }

    public async Task<bool> CreateTopic(string topicName, string displayNameValue)
    {
        var topicRequest = new CreateTopicRequest
        {
            Name = topicName
        };

        var topicResponse = await sns.CreateTopicAsync(topicRequest);

        var topicAttRequest = new SetTopicAttributesRequest
        {
            TopicArn = topicResponse.TopicArn,
            AttributeName = "DisplayName",
            AttributeValue = displayNameValue
        };

        await sns.SetTopicAttributesAsync(topicAttRequest);
        return topicResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> CreateEmailSubscription(string topicName, string endPoint)
    {
        var topicResponse = sns.FindTopicAsync(topicName);
        var subscribeRequest = new SubscribeRequest();

        subscribeRequest.TopicArn = topicResponse.Result.TopicArn;
        subscribeRequest.Protocol = "Email";
        subscribeRequest.Endpoint = endPoint;

        var response = await sns.SubscribeAsync(subscribeRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<AccountResponse[]> GetUserAsync(string email)
    {
        var request = new QueryRequest
        {
            TableName = "user-table",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":email", new AttributeValue { S = email } }
                },
            KeyConditionExpression = $"Email = :email"
        };

        var response = await dynamoDB.QueryAsync(request);
        if (response?.Items?.Any() != true)
        {
            return Array.Empty<AccountResponse>();
        }

        var users = new List<AccountResponse>();
        foreach (var item in response.Items)
        {
            item.TryGetValue("FirstName", out var fName);
            item.TryGetValue("Email", out var emailOut);

            users.Add(new AccountResponse
            {
                Email = emailOut?.S,
                FirstName = fName?.S
            });
        }
        return users.ToArray();
    }

    public async Task<bool> DeleteUserAsync(string email)
    {
        var request = new DeleteItemRequest
        {
            TableName = "user-table",
            Key = new Dictionary<string, AttributeValue>
                {
                    { ":id", new AttributeValue { S = email } }
                }
        };

        var response = await dynamoDB.DeleteItemAsync(request);

        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }


    private static string RandomString(int length)
    {
        Random random = new Random((int)DateTime.Now.Ticks);
        const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = Enumerable.Range(0, length)
            .Select(x => pool[random.Next(0, pool.Length)]);
        return new string(chars.ToArray());
    }
}