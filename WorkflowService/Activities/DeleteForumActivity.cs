using Dapr.Client;
using Dapr.Workflow;
using WorkflowService.Models;

namespace WorkflowService.Activities
{
    public class DeleteForumActivity : WorkflowActivity<DeleteForumWorkflowModel, object?>
    {

        private readonly DaprClient _daprClient;
        private readonly ILogger<DeleteForumActivity> _logger;

        public DeleteForumActivity(DaprClient daprClient, ILogger<DeleteForumActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, DeleteForumWorkflowModel input)
        {
            try
            {
                _logger.LogInformation("Deleting forum with id {ForumId}", input.ForumId);

                var message = MessageHelper.ConvertToForum(input, context.InstanceId);

                await _daprClient.PublishEventAsync("blogpubsub", "forum.delete", message);
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
