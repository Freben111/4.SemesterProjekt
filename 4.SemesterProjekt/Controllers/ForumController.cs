using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum.DTO_s;

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
            var result = await _dapr.InvokeMethodAsync<CreateForumDTO, object>(
                HttpMethod.Post,
                "forumservice",     
                "api/forum",        
                dto
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForum(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Get,
                "forumservice",
                $"api/forum/{id}"
            );

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllForums()
        {
            var result = await _dapr.InvokeMethodAsync<object[]>(
                HttpMethod.Get,
                "forumservice",
                "api/forum"
            );
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForum(Guid id, [FromBody] UpdateForumDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<UpdateForumDTO, object>(
                HttpMethod.Put,
                "forumservice",
                $"api/forum/{id}",
                dto
            );
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForum(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Delete,
                "forumservice",
                $"api/forum/{id}"
            );
            return Ok(result);
        }
    }
}
