using Dapr;
using Dapr.Client;
using ForumService.Application.Interfaces;
using ForumService.Application.ServiceDTO;
using Microsoft.AspNetCore.Mvc;

namespace ForumService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {

        private readonly ILogger<ForumController> _logger;
        private readonly IForumApplication _forumService;


        public ForumController(ILogger<ForumController> logger, IForumApplication forumService)
        {
            _logger = logger;
            _forumService = forumService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForum([FromBody] CreateForumDTO dto)
        {
            _logger.LogInformation("Creating forum with name: {ForumName}", dto.Name);
            var result = await _forumService.CreateForum(dto);
            return Ok(result);
        }
    }
}
