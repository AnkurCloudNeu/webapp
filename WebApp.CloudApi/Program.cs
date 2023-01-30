using WebApp.CloudApi.Class;
using WebApp.CloudApi.EfCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using WebApp.CloudApi.Helper;
using Amazon.S3;
using WebApp.CloudApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Amazon.Runtime;
using System.Reflection;
using NLog;
using NLog.Web;
using JustEat.StatsD;
using LogLevel = NLog.LogLevel;
using NLog.Targets;
using NLog.Config;
using NLog.AWS.Logger;
using Amazon.CloudWatch;
using AspNetCore.Aws.Demo;
using WebApp.CloudApi.DynamoDb;
using WebApp.CloudApi.Interface;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.SQS;
using Amazon.SimpleNotificationService;

// Setup the NLog configuration
var config = new LoggingConfiguration();
config.AddRule(LogLevel.Info, LogLevel.Fatal, new ConsoleTarget());

// Add the AWS Target with minimal configuration
config.AddRule(LogLevel.Info, LogLevel.Fatal, new AWSTarget()
{
    LogGroup = "/dotnet/logging-demo/nlog"
});

LogManager.Configuration = config;

// Create a new logger and test it
var log = LogManager.GetCurrentClassLogger();
log.Info("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    DotNetEnv.Env.Load("/home/ubuntu/webapp/WebApp.CloudApi/.env");
    // DotNetEnv.Env.Load();
    string connectionString = $"Host={DotNetEnv.Env.GetString("Host")};Database={DotNetEnv.Env.GetString("DatabaseName")};Port={DotNetEnv.Env.GetString("DatabasePort")};Username={DotNetEnv.Env.GetString("MasterUsername")};Password={DotNetEnv.Env.GetString("MasterPassword")};";
    builder.Services.AddDbContext<EF_DataContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });

    builder.Services.AddControllers();
    builder.Services.AddSingleton<ApplicationInstance>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });
    builder.Services.AddScoped<IDbHelper, DbHelper>();
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
    });

    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

    builder.Services.AddAWSService<IAmazonS3>();
    builder.Services.AddAWSService<IAmazonDynamoDB>();
    builder.Services.AddAWSService<IAmazonSQS>();
    builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
    builder.Services.AddTransient<IUserCreator, UserCreator>();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Cloud API",
            Description = "An ASP.NET Core Web API for Cloud Project",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Contact",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://example.com/license")
            }
        });

        Console.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    });

    builder.Services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                }
            );
    builder.Services.AddAWSService<IAmazonCloudWatch>();

    builder.Services.AddStatsD(
    (provider) =>
    {
        return new StatsDConfiguration
        {
            Host = "localhost",
            SocketProtocol = SocketProtocol.IP
        };
    });

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cloud API V1");
    });
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseMiddleware<CloudWatchExecutionTimeMiddleware>();
    app.MigrateDatabase();
    if (app.Environment.IsDevelopment())
    {
        app.Run();
    }
    else
    {
        app.Run("http://0.0.0.0:8080");
    }
}
catch (Exception exception)
{
    // NLog: catch setup errors
    log.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
