using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Post.DTO_s;
using System.Net.Http.Headers;
using System.Text.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    [Authorize]
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
            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "postservice",
                $"api/post");

            request.Headers.Authorization = authHeader;

            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
