using Dapr.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum;
using Shared.Forum.DTO_s;
using WorkflowService.Models;
using WorkflowService.Workflows;

namespace WorkflowService.Controllers
{
    [ApiController]
    [Authorize]
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

        [HttpDelete("forum/{forumId}")]
        public async Task<IActionResult> DeleteForumWorkflow(Guid forumId)
        {
            var instanceId = Guid.NewGuid().ToString();
            try
            {
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null)
                {
                    _logger.LogError("UserId claim not found");
                    return StatusCode(401, new ForumResultMessage
                    {
                        Status = "Error",
                        Error = "UserId claim not found",
                    });
                    //throw new Exception(new
                    //{
                    //    status = "Error",
                    //    statusCode = 401,
                    //    message = "UserId Claim not found"
                    //}.ToString());
                }

                await _workflowClient.ScheduleNewWorkflowAsync(
                    nameof(ForumDeletionWorkflow),
                    instanceId,
                    new DeleteForumWorkflowModel
                    {
                        ForumId = forumId,
                        JWT = Request.Headers["Authorization"].ToString()
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
                _logger.LogError(ex, "Error deleting forum with id {ForumId}", forumId);
                return StatusCode(500, new ForumResultMessage
                {
                    Status = "Error",
                    StatusCode = 500,
                    Error = ex.Message
                });
            }
        }
    }
}
