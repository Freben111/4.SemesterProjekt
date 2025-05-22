using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly DaprClient _dapr;

        public WorkflowController(DaprClient dapr)
        {
            _dapr = dapr;
        }

        [HttpDelete("forum/{forumId}")]
        public async Task<IActionResult> DeleteForum(Guid forumId)
        {
            try
            {
                if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValue) || string.IsNullOrWhiteSpace(authHeaderValue))
                {
                   throw new Exception(new
                   {
                       status = "Error",
                       statusCode = 401,
                       message = "Authorization header is missing"
                   }.ToString());
                }

                var request = _dapr.CreateInvokeMethodRequest(
                    HttpMethod.Delete,
                    "workflowservice",
                    $"api/Workflow/forum/{forumId}");

                request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeaderValue);


                var response = await _dapr.InvokeMethodWithResponseAsync(request);

                var responseContent = await response.Content.ReadFromJsonAsync<object>();

                return StatusCode((int)response.StatusCode, responseContent);
            }
            catch (Exception ex)
            {
                var responseCode = ex.Data["statuscode"];
                if (responseCode == null)
                {
                    responseCode = 600;
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
