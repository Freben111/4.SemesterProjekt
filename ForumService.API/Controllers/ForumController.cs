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
            var userIdClaim = User.FindFirst("UserId");
            if(userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized("UserId Claim not found");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var result = await _forumCommand.CreateForum(dto, userId);
            return Ok(result);

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForum(Guid id)
        {
            _logger.LogInformation("Getting forum with id: {ForumId}", id);
            var result = await _forumQuery.GetForum(id);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllForums()
        {
            _logger.LogInformation("Getting all forums");
            var result = await _forumQuery.GetAllForums();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForum(Guid id, [FromBody] UpdateForumDTO dto)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized("UserId Claim not found");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            _logger.LogInformation("Updating forum with id: {ForumId}", id);
            var result = await _forumCommand.UpdateForum(id, dto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForum(Guid id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized("UserId Claim not found");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            _logger.LogInformation("Deleting forum with id: {ForumId}", id);
            var result = await _forumCommand.DeleteForum(id, userId);
            return Ok(result);
        }
    }
}
