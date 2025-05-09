using ForumService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum.DTO_s;

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


        public ForumController(ILogger<ForumController> logger, IForumCommand forumCommand, IForumQuery forumQuery)
        {
            _logger = logger;
            _forumCommand = forumCommand;
            _forumQuery = forumQuery;
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
        public async Task<IActionResult> DeleteForum(Guid id)
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

            _logger.LogInformation("Deleting forum with id: {ForumId}", id);
            var result = await _forumCommand.DeleteForum(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
