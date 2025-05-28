using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum;
using Shared.Forum.DTO_s;
using System.Net.Http.Headers;
using System.Text.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly DaprClient _dapr;

        public ForumController(DaprClient dapr)
        {
            _dapr = dapr;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForum([FromBody] CreateForumDTO dto)
        {
            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "forumservice",
                $"api/forum");

            request.Headers.Authorization = authHeader;

            //sætter content til at være json og sætter content type til application/json
            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);

            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForum(Guid id)
        {

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Get,
                "forumservice",
                $"api/forum/{id}");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllForums()
        {
            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Get,
                "forumservice",
                $"api/forum");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForum(Guid id, [FromBody] UpdateForumDTO dto)
        {
            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Put,
                "forumservice",
                $"api/forum/{id}");

            request.Headers.Authorization = authHeader;

            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteForum(Guid id)
        //{
        //    if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
        //    {
        //        return Unauthorized("Authorization header is missing");
        //    }

        //    var request = _dapr.CreateInvokeMethodRequest(
        //        HttpMethod.Delete,
        //        "forumservice",
        //        $"api/forum/{id}");

        //    request.Headers.Authorization = authHeader;

        //    var response = await _dapr.InvokeMethodWithResponseAsync(request);
        //    var responseContent = await response.Content.ReadFromJsonAsync<object>();

        //    return StatusCode((int)response.StatusCode, responseContent);
        //}
    }
}
