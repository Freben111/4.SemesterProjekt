using Dapr.Client;
using Dapr.Workflow;
using WorkflowService.Models;

namespace WorkflowService.Activities
{
    public class DeleteCommentsActivity : WorkflowActivity<DeleteForumWorkflowModel, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<DeleteCommentsActivity> _logger;

        public DeleteCommentsActivity(DaprClient daprClient, ILogger<DeleteCommentsActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, DeleteForumWorkflowModel input)
        {
            try
            {
                _logger.LogInformation("Deleting forum with id {ForumId}", input.ForumId);

                var message = MessageHelper.ConvertToComment(input, context.InstanceId);

                await _daprClient.PublishEventAsync("blogpubsub", "Comments.Delete", message);
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
