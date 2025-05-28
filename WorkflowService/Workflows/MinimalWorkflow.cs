using Dapr.Workflow;
using Shared.Forum;
using Shared.Test;
using WorkflowService.Activities;
using WorkflowService.Models;

namespace WorkflowService.Workflows
{

    public class MinimalWorkflow : Workflow<MinimalWorkflowInput, MinimalWorkflowInput>
    {

        public override async Task<MinimalWorkflowInput> RunAsync(WorkflowContext context, MinimalWorkflowInput input)
        {
            try
            {
                await context.CallActivityAsync<MinimalWorkflowInput>(
                nameof(MinimalActivity),
                input);

                var minimalForumResult = await context.WaitForExternalEventAsync<MinimalWorkflowSharedModel>("Minimal.Forum.Test.Completed");
                if (minimalForumResult.Message != "Forum Workflow test completed successfully")
                {
                    throw new Exception($"Error in forumService");
                }

                input.Message = $"Workflow completed. Message was: {input.Message}";
                return input;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
