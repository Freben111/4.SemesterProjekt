using Dapr.Client;
using Dapr.Workflow;
using Shared.Test;
using WorkflowService.Models;

namespace WorkflowService.Activities
{

    public class MinimalActivity : WorkflowActivity<MinimalWorkflowInput, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<MinimalActivity> _logger;

        public MinimalActivity(DaprClient daprClient, ILogger<MinimalActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, MinimalWorkflowInput input)
        {
            try
            {
                input.Message = "Hello from MinimalActivity";
                _logger.LogInformation("Testing pubsub on workflow: {message}", input.Message);

                MinimalWorkflowSharedModel sharedModel = new MinimalWorkflowSharedModel
                {
                    Message = input.Message,
                    WorkFlowId = Guid.Parse(context.InstanceId)
                };

                await _daprClient.PublishEventAsync("blogpubsub", "Minimal.Forum.Test", sharedModel);
                return null;
            }
            catch (Exception ex)
            {
                input.Message = "Error in MinimalActivity";
                _logger.LogError(ex, "Error with workflow pubsub on workflow: {message}", input.Message);
                throw;
            }
        }
    }
}
