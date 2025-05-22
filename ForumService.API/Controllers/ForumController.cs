using Dapr;
using Dapr.Client;
using ForumService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum;
using Shared.Forum.DTO_s;
using System.Security.Claims;

namespace ForumService.API.Controllers
{
    [ApiController]
    [Authorize]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [Topic("pubsub", "forum.delete")]
        public async Task<IActionResult> DeleteForum(ForumMessage input)
        {
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
            var token = input.JWT.StartsWith("Bearer ") ? input.JWT.Substring(7) : input.JWT;

            var validToken = _jwtValidator.ValidateToken(token);

            var userIdClaim = validToken.FindFirst("userId");
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

            await _daprClient.PublishEventAsync("pubsub", "Forum.Deleted", result);

            return Ok();
        }

        [HttpPost("restore")]
        [Topic("pubsub", "forum.restore")]
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
            return Ok(result);
        }
    }
}
