using CommunityToolkit.Aspire.Hosting.Dapr;


var builder = DistributedApplication.CreateBuilder(args);

var statestore = builder.AddDaprStateStore("statestore");
var pubsubComponent = builder.AddDaprPubSub("pubsub");

builder.AddProject<Projects.APIGateway>("apigateway")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "apigateway",
        DaprHttpPort = 3501,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);


builder.AddProject<Projects.ForumService_API>("forumservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "forumservice",
        DaprHttpPort = 3502,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);



builder.AddProject<Projects.PostService_API>("postservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "postservice",
        DaprHttpPort = 3503,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);



builder.AddProject<Projects.CommentService_API>("commentservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "commentservice",
        DaprHttpPort = 3504,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);



builder.AddProject<Projects.UserService_API>("user-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "userservice",
        DaprHttpPort = 3505,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);



builder.AddProject<Projects.WorkflowService>("workflowservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "workflowservice",
        DaprHttpPort = 3506,
    }).WithReference(pubsubComponent)
    .WithReference(statestore);



builder.Build().Run();
