using Dapr.Client;
using ForumService.Application.ServiceDTO;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [ApiController]
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
                "forumservice",     
                "api/forum",        
                dto
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForum(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<string>(
                "forumservice",
                $"api/forum/{id}"
            );

            return Ok(result);
        }

        // Add Update and Delete similarly
    }
}
