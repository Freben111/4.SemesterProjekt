using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.User;
using Shared.User.DTO_s;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserCommand _userCommand;
        private readonly IUserQuery _userQuery;
        private readonly IJwtService _jwtService;


        public UserController(ILogger<UserController> logger, IUserCommand userCommand, IUserQuery userQuery, IJwtService jwtService)
        {
            _logger = logger;
            _userCommand = userCommand;
            _userQuery = userQuery;
            _jwtService = jwtService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] CreateUserDTO dto)
        {

            _logger.LogInformation("Creating user with name: {Username}", dto.UserName);
            var result = await _userCommand.CreateUser(dto);
            return StatusCode(result.StatusCode, result);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = new UserLoginMessage();
            _logger.LogInformation("Logging in user with name: {Username}", dto.username);
            result = await _userCommand.Login(dto);
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting user with id: {UserId}", id);
                var result = await _userQuery.GetUser(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, "Bad Request");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var result = await _userQuery.GetAllUsers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, "Bad Request");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO dto)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "UserId Claim not found"
                });
            }
            var userId = Guid.Parse(userIdClaim.Value);


            _logger.LogInformation("Updating user with id: {UserId}", id);
            var result = await _userCommand.UpdateUser(id, dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized("UserId Claim not found");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            _logger.LogInformation("Deleting user with id: {UserId}", id);
            var result = await _userCommand.DeleteUser(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
