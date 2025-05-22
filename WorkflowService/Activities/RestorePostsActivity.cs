using Dapr.Client;
using Dapr.Workflow;
using Shared.Post;

namespace WorkflowService.Activities
{
    public class RestorePostsActivity : WorkflowActivity<PostResultMessage, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<RestorePostsActivity> _logger;

        public RestorePostsActivity(DaprClient daprClient, ILogger<RestorePostsActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, PostResultMessage input)
        {
            try
            {
                _logger.LogInformation("Restoring posts");

                await _daprClient.PublishEventAsync("pubsub", "posts.restore", input.BackupPost);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring posts");
                throw;
            }
        }
    }
}
