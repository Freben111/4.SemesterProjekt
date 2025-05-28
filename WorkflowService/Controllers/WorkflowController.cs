using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment;
using Shared.Forum;
using Shared.Forum.DTO_s;
using Shared.Post;
using Shared.Test;
using WorkflowService.Models;
using WorkflowService.Workflows;

namespace WorkflowService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {

        private readonly ILogger<WorkflowController> _logger;
        private readonly DaprWorkflowClient _workflowClient;
        public WorkflowController(ILogger<WorkflowController> logger, DaprWorkflowClient WorkflowClient)
        {
            _logger = logger;
            _workflowClient = WorkflowClient;
        }

        [HttpPost("minimal")]
        public async Task<IActionResult> StartMinimalWorkflow([FromBody] string input)
        {
            var instanceId = Guid.NewGuid().ToString();

            var inputModel = new MinimalWorkflowInput
            {
                Message = input,
                WorkFlowId = Guid.Parse(instanceId)
            };

            _logger.LogInformation("Started minimal workflow with instance id: {InstanceId}", instanceId);

            await _workflowClient.ScheduleNewWorkflowAsync(
                nameof(MinimalWorkflow),
                instanceId,
                inputModel);

            return Ok(new { StatusCode = 200, Status = "Started", WorkflowId = instanceId });
        }

        [HttpDelete("forum/{forumId}")]
        public async Task<IActionResult> DeleteForumWorkflow([FromBody] ForumMessage dto)
        {
            var instanceId = Guid.NewGuid().ToString();
            try
            {
                _logger.LogInformation("Starting workflow to delete forum with id: {ForumId}", dto.ForumId);


                if (dto.JWT == null)
                {
                    _logger.LogError("UserId claim not found");
                    return StatusCode(401, new ForumResultMessage
                    {
                        Status = "Error",
                        Error = "UserId claim not found",
                    });
                }

                await _workflowClient.ScheduleNewWorkflowAsync(
                    nameof(ForumDeletionWorkflow),
                    instanceId,
                    new DeleteForumWorkflowModel
                    {
                        ForumId = Guid.Parse(dto.ForumId),
                        JWT = dto.JWT
                    });

                _logger.LogInformation("Started workflow with instance id: {InstanceId}", instanceId);
                return StatusCode(200, new ForumResultMessage
                {
                    
                    Status = "Success, workflow startet correctly",
                    WorkflowId = instanceId
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting forum with id {ForumId}", dto.ForumId);
                return StatusCode(500, new ForumResultMessage
                {
                    Status = "Error",
                    StatusCode = 500,
                    Error = ex.Message
                });
            }
        }


        //workflow controller skal modetage en pubsub fra forumservice når den er færdig og håndtere pubsub her og til sidst kalde raiseEventAsyns til workflowet så det kan komme videre

        [HttpPost("forum-status")]
        [Topic("blogpubsub", "Forum.Deleted")]
        public async Task<IActionResult> ForumStatus(ForumResultMessage message)
        {
            if (message.JWT == null || string.IsNullOrEmpty(message.JWT))
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");

            await _workflowClient.RaiseEventAsync(message.WorkflowId ,"ForumDeleted", message);
            return Ok(message);
        }

        [HttpPost("post-status")]
        [Topic("blogpubsub", "Posts.Deleted")]
        public async Task<IActionResult> PostStatus(PostResultMessage message)
        {
            if (message.JWT == null || string.IsNullOrEmpty(message.JWT))
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");

            await _workflowClient.RaiseEventAsync(message.WorkflowId, "PostsDeleted", message);
            return Ok(message);
        }

        [HttpPost("comment-status")]
        [Topic("blogpubsub", "Comments.Deleted")]
        public async Task<IActionResult> CommentStatus(CommentResultMessage message)
        {
            if (message.JWT == null || string.IsNullOrEmpty(message.JWT))
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");

            await _workflowClient.RaiseEventAsync(message.WorkflowId, "CommentsDeleted", message);
            return Ok(message);
        }

        [HttpPost("MinimalTest")]
        [Topic("blogpubsub", "Minimal.Test.PubSub.Return")]
        public async Task<IActionResult> MinimalTest(MinimalWorkflowSharedModel input)
        {
            if (input.Message == null || string.IsNullOrEmpty(input.Message))
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation("Recived pubsub from forum");

            input.Message = "Forum Workflow test completed successfully";
            try
            {
                await _workflowClient.RaiseEventAsync(input.WorkFlowId.ToString(), "Minimal.Forum.Test.Completed", input);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation("workflow event raise");
            return Ok(new { StatusCode = 200, Status = "Started", WorkflowId = input.WorkFlowId });
        }
    }
}
