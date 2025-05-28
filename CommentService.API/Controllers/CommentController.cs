using CommentService.Application.Interfaces;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment;
using Shared.Comment.DTO_s;
using Shared.Forum;

namespace CommentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {

        private readonly ILogger<CommentController> _logger;
        private readonly ICommentCommand _commentCommand;
        private readonly ICommentQuery _commentQuery;
        private readonly DaprClient _daprClient;
        private readonly IJwtValidator _jwtValidator;


        public CommentController(ILogger<CommentController> logger, ICommentCommand CommentCommand, ICommentQuery CommentQuery, DaprClient daprClient, IJwtValidator jwtValidator)
        {
            _logger = logger;
            _commentCommand = CommentCommand;
            _commentQuery = CommentQuery;
            _daprClient = daprClient;
            _jwtValidator = jwtValidator;
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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

        [HttpPost("post/workflow")]
        [AllowAnonymous]
        [Topic("blogpubsub", "Comments.Delete")]
        public async Task<IActionResult> DeleteCommentByPostId(CommentMessage input)
        {
            _logger.LogInformation("Deleting comments with from different posts");

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
            var result = await _commentCommand.DeleteCommentByPostIds(input.PostIds, userId);
            result.JWT = input.JWT;
            result.WorkflowId = input.WorkflowId;

            await _daprClient.PublishEventAsync("blogpubsub", "Comments.Deleted", result);

            return Ok();
        }

    }
}
