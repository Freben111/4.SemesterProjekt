using Dapr;
using Dapr.Client;
using ForumService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum;
using Shared.Forum.DTO_s;
using Shared.Test;
using System.Security.Claims;

namespace ForumService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {

        private readonly ILogger<ForumController> _logger;
        private readonly IForumCommand _forumCommand;
        private readonly IForumQuery _forumQuery;
        private readonly IJwtValidator _jwtValidator;
        private readonly DaprClient _daprClient;


        public ForumController(ILogger<ForumController> logger, IForumCommand forumCommand, IForumQuery forumQuery, IJwtValidator jwtValidator, DaprClient daprClient)
        {
            _logger = logger;
            _forumCommand = forumCommand;
            _forumQuery = forumQuery;
            _jwtValidator = jwtValidator;
            _daprClient = daprClient;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateForum([FromBody] CreateForumDTO dto)
        {
            
            _logger.LogInformation("Creating forum with name: {ForumName}", dto.Name);
            var userIdClaim = User.FindFirst("userId");
            if(userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "UserId Claim not found"
                });
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var result = await _forumCommand.CreateForum(dto, userId);
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForum(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting forum with id: {ForumId}", id);
                var result = await _forumQuery.GetForum(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(400, $"Bad Request: {ex}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForums()
        {
            try
            {
                _logger.LogInformation("Getting all forums");
                var result = await _forumQuery.GetAllForums();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"Bad Request{ex}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateForum(Guid id, [FromBody] UpdateForumDTO dto)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "UserId Claim not found"
                });
            }
            var userId = Guid.Parse(userIdClaim.Value);

            _logger.LogInformation("Updating forum with id: {ForumId}", id);
            var result = await _forumCommand.UpdateForum(id, dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("workflow")]
        [Topic("blogpubsub", "forum.delete")]
        public async Task<IActionResult> DeleteForum([FromBody]ForumMessage input)
        {
            _logger.LogInformation(input.ForumId, input.ForumName, input.OwnerId, input.WorkflowId, input.JWT);
            if (Request.Headers.TryGetValue("ce-type", out var ceType) &&
                ceType == "com.dapr.event.subscription.validation")
            {
                _logger.LogInformation("Dapr subscription validation event received, ignoring.");
                return Ok();
            }
            _logger.LogInformation("Received request to delete forum with id: {ForumId}", input.ForumId);
            if (input.JWT == null)
            {
                _logger.LogError("Token not found");
                return Unauthorized(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "Token not found"
                });
            }
            _logger.LogInformation("Validating JWT token");
            var token = input.JWT.StartsWith("Bearer ") ? input.JWT.Substring(7) : input.JWT;

            var validToken = _jwtValidator.ValidateToken(token);

            var userIdClaim = validToken?.FindFirst("userId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "UserId Claim not found"
                });
            }
            var userId = Guid.Parse(userIdClaim.Value);

            _logger.LogInformation("Deleting forum with id: {ForumId}", input.ForumId);
            var result = await _forumCommand.DeleteForum(Guid.Parse(input.ForumId), userId);
            result.JWT = input.JWT;
            result.WorkflowId = input.WorkflowId;

            await _daprClient.PublishEventAsync("blogpubsub", "Forum.Deleted", result);
            return Ok();
        }

        [HttpPost("restore")]
        [Topic("blogpubsub", "forum.restore")]
        public async Task<IActionResult> RestoreForum(ForumBackupDTO backup)
        {
            //if (input.JWT == null)
            //{
            //    _logger.LogError("Token not found");
            //    return Unauthorized(new
            //    {
            //        status = "Error",
            //        statusCode = 401,
            //        message = "Token not found"
            //    });
            //}
            //var token = input.JWT.StartsWith("Bearer ") ? input.JWT.Substring(7) : input.JWT;
            //var validToken = _jwtValidator.ValidateToken(token);
            //var userIdClaim = validToken.FindFirst("userId");
            //if (userIdClaim == null)
            //{
            //    _logger.LogError("UserId claim not found");
            //    return Unauthorized(new
            //    {
            //        status = "Error",
            //        statusCode = 401,
            //        message = "UserId Claim not found"
            //    });
            //}
            //var userId = Guid.Parse(userIdClaim.Value);
            //_logger.LogInformation("Restoring forum with id: {ForumId}", input.ForumId);
            var result = await _forumCommand.RestoreForum(backup);

            await _daprClient.PublishEventAsync("blogpubsub", "Forum.Restored", result);
            
            return Ok(result);
        }

        [HttpPost("workflow/test")]
        [Topic("blogpubsub", "Minimal.Forum.Test")]
        [AllowAnonymous]
        public async Task<IActionResult> WorkflowTest(MinimalWorkflowSharedModel input)
        {
            if (input.Message == null || string.IsNullOrEmpty(input.Message))
            {
                _logger.LogInformation("Received empty or test event, ignoring.");
                return Ok();
            }
            _logger.LogInformation("ForumService workflow test, workflow message {message}", input.Message);
            await Task.Delay(1000);

            input.Message = "Forum Workflow test completed successfully";
            _logger.LogInformation("Publishing workflow test completed message: {message}", input.Message);


            await _daprClient.PublishEventAsync("blogpubsub", "Minimal.Test.PubSub.Return", input);
            _logger.LogInformation("Workflow test completed with message: {message}", input.Message);
            return Ok();

        }
    }
}
