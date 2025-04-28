using CommunityToolkit.Aspire.Hosting.Dapr;


var builder = DistributedApplication.CreateBuilder(args);

//var statestore = builder.AddDaprStateStore("blogstatestore");
//var pubsubComponent = builder.AddDaprPubSub("blogpubsub");

builder.AddProject<Projects.APIGateway>("apigateway")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "apigateway",
        DaprHttpPort = 3501,
    });


builder.AddProject<Projects.ForumService_API>("forumservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "forumservice",
        DaprHttpPort = 3502,
    });



builder.AddProject<Projects.PostService_API>("postservice-api")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "postservice",
        DaprHttpPort = 3503,
    });



builder.Build().Run();
