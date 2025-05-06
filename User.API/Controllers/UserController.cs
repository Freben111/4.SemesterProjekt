using UserService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.User.DTO_s;
using Microsoft.AspNetCore.Authorization;

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
            return Ok(result);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            _logger.LogInformation("Logging in user with name: {Username}", dto.username);
            var result = await _userCommand.Login(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("Getting user with id: {UserId}", id);
            var result = await _userQuery.GetUser(id);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Getting all users");
            var result = await _userQuery.GetAllUsers();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO dto)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                _logger.LogError("UserId claim not found");
                return Unauthorized("UserId Claim not found");
            }
            var userId = Guid.Parse(userIdClaim.Value);


            _logger.LogInformation("Updating user with id: {UserId}", id);
            var result = await _userCommand.UpdateUser(id, dto, userId);
            return Ok(result);
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
            return Ok(result);
        }
    }
}
