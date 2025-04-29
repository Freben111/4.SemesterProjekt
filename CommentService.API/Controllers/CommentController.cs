using CommentService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment.DTO_s;

namespace CommentService.API.Controllers
{
    [ApiController]
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
            var result = await _commentCommand.CreateComment(dto);
            return Ok(result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            _logger.LogInformation("Getting comment with id: {CommentId}", id);
            var result = await _commentQuery.GetComment(id);
            return Ok(result);
        }

        [HttpGet]
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
            var result = await _commentCommand.UpdateComment(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            _logger.LogInformation("Deleting Comment with id: {CommentId}", id);
            var result = await _commentCommand.DeleteComment(id);
            return Ok(result);
        }
    }
}
