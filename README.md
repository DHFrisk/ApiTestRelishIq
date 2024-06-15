# Table of Contents  
[Summary](#summary)

[NuGet packages](#nuget-packages)

[Basic diagram](#basic-diagram)

[POC](#poc)

[Links](#links)

# Summary
The projects are done with .NET 8, there are 4 directories containing source code:
- ApiTestRelishIq (Local API project): Requests data from the provided endpoints and returns it as the desired JSON object(s).
- WebpageTestRelishIq (MVC project): Basically a very simple two webpages frontend site, it can be run locally or deployed in a cloud provider (currently running on a AWS Beanstalk instance). The endpoint API URL is hardcoded here.
- AwsLambdaServerlessApi (AWS API project): Basically an adapted project ready to be deployed to an AWS Lambda Function, it has the same core logic of *ApiTestRelishIq* adding the necessary stuff to handle it with Lambda Functions.
- Tests: A single unit test, just made to try something while I was developing.

# NuGet packages
Here are the installed NuGet packages:

```
Project 'ApiTestRelishIq' has the following package references
   [net8.0]:
   Top-level Package                                       Requested   Resolved
   > Microsoft.AspNetCore.Mvc.NewtonsoftJson               8.0.6       8.0.6
   > Microsoft.AspNetCore.OpenApi                          8.0.4       8.0.4
   > Microsoft.VisualStudio.Web.CodeGeneration.Design      8.0.2       8.0.2
   > Swashbuckle.AspNetCore                                6.5.0       6.5.0

Project 'WebpageTestRelishIq' has the following package references
   [net8.0]:
   Top-level Package                                       Requested   Resolved
   > Amazon.Lambda.APIGatewayEvents                        2.7.0       2.7.0
   > Amazon.Lambda.Core                                    2.2.0       2.2.0
   > Amazon.Lambda.Serialization.Json                      2.2.1       2.2.1
   > Amazon.Lambda.Serialization.SystemTextJson            2.4.3       2.4.3
   > Microsoft.VisualStudio.Web.CodeGeneration.Design      8.0.2       8.0.2

Project 'Tests' has the following package references
   [net8.0]:
   Top-level Package             Requested   Resolved
   > coverlet.collector          6.0.0       6.0.0
   > Microsoft.NET.Test.Sdk      17.8.0      17.8.0
   > NUnit                       3.14.0      3.14.0
   > NUnit.Analyzers             3.9.0       3.9.0
   > NUnit3TestAdapter           4.5.0       4.5.0

Project 'AwsLambdaServerlessApi' has the following package references
   [net8.0]:
   Top-level Package                                 Requested   Resolved
   > Amazon.Lambda.APIGatewayEvents                  2.7.0       2.7.0
   > Amazon.Lambda.AspNetCoreServer                  9.0.0       9.0.0
   > Amazon.Lambda.Serialization.Json                2.2.1       2.2.1
   > Amazon.Lambda.Serialization.SystemTextJson      2.4.3       2.4.3
   > Amazon.S3                                       0.33.0      0.33.0
   > AWSSDK.S3                                       3.7.309.4   3.7.309.4
```

# Basic diagram
![Very basic diagram](https://github.com/DHFrisk/ApiTestRelishIq/blob/master/GifsAndImages/TestRelishIq.drawio.png)

# POC
## MVC hosted locally
I know is a very basic and somehow ugly UX/UI :^)
![MVC hosted locally](https://github.com/DHFrisk/ApiTestRelishIq/blob/master/GifsAndImages/MvcRunningLocally.gif)

## MVC hosted on AWS Beanstalk
Sadly when the project is deployed to the AWS Beanstalk instance it doesn't load the styles :|
![MVC hosted on AWS Beanstalk](https://github.com/DHFrisk/ApiTestRelishIq/blob/master/GifsAndImages/MvcRunningOnBeanstalk.gif)

# Links
- API: https://p6ax5xqqwa2vomfz4ytwarfozy0usihb.lambda-url.us-east-1.on.aws/photos
- MVC: http://test-relishiq-env.eba-pi2umfz5.us-east-1.elasticbeanstalk.com/

