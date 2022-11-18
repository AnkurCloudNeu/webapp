using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using WebApp.CloudApi.Interface;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.DynamoDb;

public class UserCreator : IUserCreator
{
    private readonly IAmazonDynamoDB dynamoDB;
    private readonly IAmazonSQS sqs;
    private readonly IAmazonSimpleNotificationService sns;
    public UserCreator(IAmazonDynamoDB dynamoDB, IAmazonSQS sqs, IAmazonSimpleNotificationService sns)
    {
        this.dynamoDB = dynamoDB;
        this.sqs = sqs;
        this.sns = sns;
    }
    public async Task<bool> CreateUser(AccountResponse user)
    {
        try
        {
            var request = new PutItemRequest
            {
                TableName = "user-table",
                Item = new Dictionary<string, AttributeValue> {
                {"Email", new AttributeValue(user.Email) },
                {"FirstName", new AttributeValue(user.FirstName) }
            }
            };
            var response = await dynamoDB.PutItemAsync(request);
            var result = response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            if (result) {
                var message = new SendMessageRequest {
                    QueueUrl = "https://sqs.us-east-1.amazonaws.com/429484779684/demo-queue",
                    MessageBody = $"Hello from user with {user.Email}"
                };
                await sqs.SendMessageAsync(message);
                await CreateEmailSubscription("test-topic", user.Email);
            }
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> CreateTopic(string topicName, string displayNameValue) {
        var topicRequest = new CreateTopicRequest {
            Name = topicName
        };

        var topicResponse = await sns.CreateTopicAsync(topicRequest);

        var topicAttRequest = new SetTopicAttributesRequest {
            TopicArn = topicResponse.TopicArn,
            AttributeName = "DisplayName",
            AttributeValue = displayNameValue 
        };

        await sns.SetTopicAttributesAsync(topicAttRequest);
        return topicResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> CreateEmailSubscription(string topicName, string endPoint) {
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
                    { ":email", new AttributeValue { S = email } }
                }
        };

        var response = await dynamoDB.DeleteItemAsync(request);

        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}