using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using Shared.User.DTO_s;
using Microsoft.AspNetCore.Authorization;
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
            var result = await _dapr.InvokeMethodAsync<CreateUserDTO, object>(
                HttpMethod.Post,
                "userservice",
                "api/user",
                dto
            );
            return Ok(result);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _dapr.InvokeMethodAsync<LoginDTO, object>(
                HttpMethod.Post,
                "userservice",
                "api/user/login",
                dto
            );
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _dapr.InvokeMethodAsync<object>(
                HttpMethod.Get,
                "userservice",
                $"api/user/{id}"
            );
            return Ok(result);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _dapr.InvokeMethodAsync<object[]>(
                HttpMethod.Get,
                "userservice",
                "api/user"
            );
            return Ok(result);
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
            return Ok(response);
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
            return Ok(response);


            //Det er gjort sådan for at undgå at ændre på headeren. Bearer problemet
        }
    }
}
