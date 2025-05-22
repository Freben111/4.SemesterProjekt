using Dapr.Client;
using Dapr.Workflow;
using Shared.Forum;
using WorkflowService.Models;

namespace WorkflowService.Activities
{
    public class RestoreForumActivity : WorkflowActivity<ForumResultMessage, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<RestoreForumActivity> _logger;

        public RestoreForumActivity(DaprClient daprClient, ILogger<RestoreForumActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, ForumResultMessage input)
        {
            try
            {
                _logger.LogInformation("Restoring forum with id {ForumId}", input.ForumId);

                await _daprClient.PublishEventAsync("pubsub", "forum.restore", input.BackupForum);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring forum with id {ForumId}", input.ForumId);
                throw;
            }
        }
    }
}
