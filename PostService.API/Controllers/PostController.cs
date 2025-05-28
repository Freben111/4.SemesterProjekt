using PostService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Post.DTO_s;
using Shared.Post;
using Dapr;
using Shared.Forum;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;

namespace PostService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {

        private readonly ILogger<PostController> _logger;
        private readonly IPostCommand _postCommand;
        private readonly IPostQuery _postQuery;
        private readonly DaprClient _daprClient;
        private readonly IJwtValidator _jwtValidator;


        public PostController(ILogger<PostController> logger, IPostCommand postCommand, IPostQuery postQuery, DaprClient daprClient, IJwtValidator jwtValidator)
        {
            _logger = logger;
            _postCommand = postCommand;
            _postQuery = postQuery;
            _daprClient = daprClient;
            _jwtValidator = jwtValidator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO dto)
        {
            _logger.LogInformation("Creating post with name: {PostTitle}", dto.Title);
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
            var result = await _postCommand.CreatePost(dto, userId);
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {

            _logger.LogInformation("Getting post with id: {PostId}", id);
            var result = await _postQuery.GetPost(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            _logger.LogInformation("Getting all posts");
            var result = await _postQuery.GetAllPosts();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostDTO dto)
        {
            _logger.LogInformation("Updating post with id: {PostId}", id);
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
            var result = await _postCommand.UpdatePost(id, dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            _logger.LogInformation("Deleting post with id: {PostId}", id);
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
            var result = await _postCommand.DeletePost(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("forum/workflow")]
        [Topic("blogpubsub", "Posts.Delete")]
        public async Task<IActionResult> DeletePostsFromForum(PostMessage input)
        {
            _logger.LogInformation("Deleting post with forumid: {forumId}", input.ForumId);

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
            var result = await _postCommand.DeletePostFromForum(input.ForumId ?? Guid.Empty, userId);
            result.JWT = input.JWT;
            result.WorkflowId = input.WorkflowId;

            await _daprClient.PublishEventAsync("blogpubsub", "Posts.Deleted", result);

            return Ok();
        }

        [HttpPost("restore")]
        [Topic("blogpubsub", "posts.restore")]
        public async Task<IActionResult> RestorePostsFromForum(List<PostBackupDTO> backups)
        {
            //_logger.LogInformation("Restoring posts with forumid: {forumId}", input.ForumId);
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
            var result = await _postCommand.RestorePost(backups);
            return Ok(result);
        }
    }
}
