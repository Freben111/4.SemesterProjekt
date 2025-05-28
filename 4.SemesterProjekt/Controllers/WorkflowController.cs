using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Forum;
using Shared.Forum.DTO_s;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly ILogger<WorkflowController> _logger;
        private readonly DaprClient _dapr;

        public WorkflowController(DaprClient dapr, ILogger<WorkflowController> logger)
        {
            _dapr = dapr;
            _logger = logger;
        }

        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<IActionResult> WorkflowTest(string input)
        {
            try
            {
                var request = _dapr.CreateInvokeMethodRequest(
                    HttpMethod.Post,
                    "workflowservice",
                    "api/Workflow/minimal");

                var json = JsonSerializer.Serialize(input);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _dapr.InvokeMethodWithResponseAsync(request);

                var responseContent = await response.Content.ReadFromJsonAsync<object>();

                return StatusCode((int)response.StatusCode, responseContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Error",
                    statusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpDelete("forum/{forumId}")]
        public async Task<IActionResult> DeleteForum(Guid forumId)
        {
            try
            {

                    
                if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
                {
                   throw new Exception(new
                   {
                       status = "Error",
                       statusCode = 401,
                       message = "Authorization header is missing"
                   }.ToString());
                }
                var dto = new ForumMessage
                {
                    ForumId = forumId.ToString(),
                    JWT = Request.Headers["Authorization"].ToString()
                };

                var request = _dapr.CreateInvokeMethodRequest(
                    HttpMethod.Delete,
                    "workflowservice",
                    $"api/workflow/forum/{forumId}");

                request.Headers.Authorization = authHeader;

                var json = JsonSerializer.Serialize(dto);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _dapr.InvokeMethodWithResponseAsync(request);
                try
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<object>();
                    return StatusCode((int)response.StatusCode, responseContent);
                }
                catch (JsonException)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        Message = response.Content.ReadAsStringAsync().Result,
                    });
                }

            }
            catch (Exception ex)
            {
                var responseCode = ex.Data["statuscode"];
                if (responseCode == null)
                {
                    responseCode = 500;
                }
                return StatusCode((int)responseCode, new
                {
                    status = "Error",
                    statusCode = responseCode,
                    message = ex.Message
                });
            }
        }
    }
}
