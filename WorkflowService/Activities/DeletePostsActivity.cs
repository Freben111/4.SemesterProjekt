using Dapr.Client;
using Dapr.Workflow;
using WorkflowService.Models;

namespace WorkflowService.Activities
{
    public class DeletePostsActivity : WorkflowActivity<DeleteForumWorkflowModel, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<DeletePostsActivity> _logger;

        public DeletePostsActivity(DaprClient daprClient, ILogger<DeletePostsActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, DeleteForumWorkflowModel input)
        {
            try
            {
                _logger.LogInformation("Deleting posts associated with forum {ForumId}", input.ForumId);

                var message = MessageHelper.ConvertToPost(input);

                await _daprClient.PublishEventAsync("pubsub", "posts.delete", message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting forum with id {ForumId}", input.ForumId);
                throw;
            }
        }
    }
}

