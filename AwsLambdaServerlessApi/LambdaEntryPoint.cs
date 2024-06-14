using Amazon.Lambda.APIGatewayEvents;
using AwsLambdaServerlessApi.Controllers;
using System.Text.Json;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AwsLambdaServerlessApi;

/// <summary>
/// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
/// actual Lambda function entry point. The Lambda handler field should be set to
/// 
/// AwsLambdaServerlessApi::AwsLambdaServerlessApi.LambdaEntryPoint::FunctionHandlerAsync
/// </summary>
public class LambdaEntryPoint :

    // The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
    // will fail to convert the incoming request correctly into a valid ASP.NET Core request.
    //
    // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    // API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    // API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
    // Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
    // 
    // Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
    // will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.

    Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
{
    /// <summary>
    /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
    /// needs to be configured in this method using the UseStartup<>() method.
    /// </summary>
    /// <param name="builder">The IWebHostBuilder to configure.</param>
    protected override void Init(IWebHostBuilder builder)
    {
        builder
            .UseStartup<Startup>();
    }

    /// <summary>
    /// Use this override to customize the services registered with the IHostBuilder. 
    /// 
    /// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
    /// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
    /// </summary>
    /// <param name="builder">The IHostBuilder to configure.</param>
    protected override void Init(IHostBuilder builder)
    {
    }

    public async Task<APIGatewayProxyResponse> Listener(APIGatewayProxyRequest request)
    {
        TestRelishIqController controller = new TestRelishIqController();
        Task<string> response;
        string x;

        try
        {
            string[] path = request.RequestContext.Path.Split("/");
            int photoId = Convert.ToInt32(path[path.Count() - 1]);
            response = controller.Get(photoId);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(response)
            };        
        }
        catch (Exception ex)
        {
        }

        string photoTitle = request.QueryStringParameters.TryGetValue("title", out x) ? request.QueryStringParameters["title"].ToString() : "";
        string albumTitle = request.QueryStringParameters.TryGetValue("album.title", out x) ? request.QueryStringParameters["album.title"].ToString() : "";
        string userEmail = request.QueryStringParameters.TryGetValue("album.user.email", out x) ? request.QueryStringParameters["album.user.email"].ToString() : "";
        int limit = request.QueryStringParameters.TryGetValue("limit", out x) ? Convert.ToInt32(request.QueryStringParameters["limit"]) : 25;
        int offset = request.QueryStringParameters.TryGetValue("offset", out x) ? Convert.ToInt32(request.QueryStringParameters["offset"]) : 0;
        
        response = controller.Get(photoTitle, albumTitle, userEmail, limit, offset);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response)
        };
    }

}