using CommentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment.DTO_s;

namespace CommentService.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {

        private readonly ILogger<CommentController> _logger;
        private readonly ICommentCommand _commentCommand;
        private readonly ICommentQuery _commentQuery;


        public CommentController(ILogger<CommentController> logger, ICommentCommand CommentCommand, ICommentQuery CommentQuery)
        {
            _logger = logger;
            _commentCommand = CommentCommand;
            _commentQuery = CommentQuery;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO dto)
        {

            _logger.LogInformation("Creating comment on post: {PostId}", dto.PostId);
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
            var result = await _commentCommand.CreateComment(dto, userId);
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComment(Guid id)
        {
            _logger.LogInformation("Getting comment with id: {CommentId}", id);
            var result = await _commentQuery.GetComment(id);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllComments()
        {
            _logger.LogInformation("Getting all comments");
            var result = await _commentQuery.GetAllComments();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDTO dto)
        {
            _logger.LogInformation("Updating comment with id: {CommentId}", id);
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
            var result = await _commentCommand.UpdateComment(id, dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            _logger.LogInformation("Deleting Comment with id: {CommentId}", id);
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
            var result = await _commentCommand.DeleteComment(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
