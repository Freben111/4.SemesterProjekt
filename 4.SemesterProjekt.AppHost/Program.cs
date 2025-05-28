using CommunityToolkit.Aspire.Hosting.Dapr;


var builder = DistributedApplication.CreateBuilder(args);

var resourcesPath = "../Resources";
//var statestore = builder.AddDaprStateStore("blogstatestore");
//var pubsubComponent = builder.AddDaprPubSub("blogpubsub");

builder.AddProject<Projects.APIGateway>("apigateway")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "apigateway",
        DaprHttpPort = 3501,
        ResourcesPaths = [resourcesPath]
    });
//.WithReference(pubsubComponent)
//.WithReference(statestore);


builder.AddProject<Projects.ForumService_API>("forumservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "forumservice",
        DaprHttpPort = 3502,
        ResourcesPaths = [resourcesPath]
    });



builder.AddProject<Projects.PostService_API>("postservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "postservice",
        DaprHttpPort = 3503,
        ResourcesPaths = [resourcesPath]
    });



builder.AddProject<Projects.CommentService_API>("commentservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "commentservice",
        DaprHttpPort = 3504,
        ResourcesPaths = [resourcesPath]
    });



builder.AddProject<Projects.UserService_API>("user-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "userservice",
        DaprHttpPort = 3505,
        ResourcesPaths = [resourcesPath]
    });



builder.AddProject<Projects.WorkflowService>("workflowservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "workflowservice",
        DaprHttpPort = 3506,
        ResourcesPaths = [resourcesPath]
    });



builder.Build().Run();
