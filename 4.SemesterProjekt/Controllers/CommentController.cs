using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment.DTO_s;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly DaprClient _dapr;

        public CommentController(DaprClient dapr)
        {
            _dapr = dapr;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<CreateCommentDTO, object>(
                HttpMethod.Post,
                "commentservice",
                "api/comment",
                dto
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Get,
                "commentservice",
                $"api/comment/{id}"
            );

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var result = await _dapr.InvokeMethodAsync<object[]>(
                HttpMethod.Get,
                "commentservice",
                "api/comment"
            );
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<UpdateCommentDTO, object>(
                HttpMethod.Put,
                "commentservice",
                $"api/comment/{id}",
                dto
            );
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Delete,
                "commentservice",
                $"api/comment/{id}"
            );
            return Ok(result);
        }
    }
}
