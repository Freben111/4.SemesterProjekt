using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Comment.DTO_s;
using System.Net.Http.Headers;
using System.Text.Json;

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
            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "commentservice",
                $"api/comment");

            request.Headers.Authorization = authHeader;

            //sætter content til at være json og sætter content type til application/json
            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);

            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
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
