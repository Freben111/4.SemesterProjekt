using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Post.DTO_s;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DaprClient _dapr;

        public PostController(DaprClient dapr)
        {
            _dapr = dapr;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<CreatePostDTO, object>(
                HttpMethod.Post,
                "postservice",
                "api/post",
                dto
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Get,
                "postservice",
                $"api/post/{id}"
            );

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _dapr.InvokeMethodAsync<object[]>(
                HttpMethod.Get,
                "postservice",
                "api/post"
            );
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<UpdatePostDTO, object>(
                HttpMethod.Put,
                "postservice",
                $"api/post/{id}",
                dto
            );
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Delete,
                "postservice",
                $"api/post/{id}"
            );
            return Ok(result);
        }
    }
}
