using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.User;
using Shared.User.DTO_s;
using System.Net.Http.Headers;
using System.Text.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DaprClient _dapr;
        public UserController(DaprClient dapr)
        {
            _dapr = dapr;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] CreateUserDTO dto)
        {

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "userservice",
                "api/user");


            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {

            var request = _dapr.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "userservice",
            "api/user/login");


            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserById(Guid id)
        {

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Get,
                "userservice",
                $"api/user/{id}");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Get,
                "userservice",
                $"api/user");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object[]>();

            return StatusCode((int)response.StatusCode, responseContent);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO dto)
        {


            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }

            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Put,
                "userservice",
                $"api/user/{id}");

            request.Headers.Authorization = authHeader;

            //sætter content til at være json og sætter content type til application/json
            var json = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);


        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            //prøver at parse Request.Headers.Authorization til AuthenticationHeaderValue
            if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                return Unauthorized("Authorization header is missing");
            }
            //Laver en ny request
            var request = _dapr.CreateInvokeMethodRequest(
                HttpMethod.Delete,
                "userservice",
                $"api/user/{id}");
            //sætter nye requests headers til authHeader aka sender jwt videre i systemet
            request.Headers.Authorization = authHeader;

            //invoker nye request
            var response = await _dapr.InvokeMethodWithResponseAsync(request);
            var responseContent = await response.Content.ReadFromJsonAsync<object>();

            return StatusCode((int)response.StatusCode, responseContent);


            //Det er gjort sådan for at undgå at ændre på headeren. Bearer problemet
        }
    }
}
