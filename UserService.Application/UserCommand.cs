using Dapr.Client;
using UserService.Application.Interfaces;
using UserService.Domain;
using UserService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.User;
using Shared.User.DTO_s;

namespace UserService.Application
{
    public class UserCommand : IUserCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<UserCommand> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUserStateModelCommand _userStateModel;

        public UserCommand(DaprClient daprClient, ILogger<UserCommand> logger, IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtService jwtService, IUserStateModelCommand userStateModel)
        {
            _daprClient = daprClient;
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _userStateModel = userStateModel;
        }

        async Task<UserResultMessage> IUserCommand.CreateUser(CreateUserDTO dto)
        {
            var result = new UserResultMessage
            {
                UserName = dto.UserName,
                Status = "Creating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingCachUser = _daprClient.GetStateAsync<UserStateModelCommand>("blogstatestore", dto.UserName).Result;
                if (existingCachUser != null)
                {
                    result.StatusCode = 409;
                    throw new Exception("User already exists");
                }
                var existingUser = await _userRepository.GetUserByUsername(dto.UserName);
                if (existingUser != null)
                {
                    result.StatusCode = 409;
                    throw new Exception("User already exists");
                }

                var hashedPassword = _passwordHasher.HashPassword(dto.Password);
                var user = User.CreateUser(dto.UserName, dto.Email, hashedPassword, dto.DisplayName);

                await _userRepository.CreateUser(user);
                await _unitOfWork.CommitAsync();

                var cachingUser = _userStateModel.Create(user);
                await _daprClient.SaveStateAsync("blog  ", user.Id.ToString(), cachingUser);

                _logger.LogInformation("User {UserName} created successfully", dto.UserName);
                result.UserName = user.UserName;
                result.Status = "Created";
                result.StatusCode = 201;
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating user {UserName}", dto.UserName);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }


        async Task<UserLoginMessage> IUserCommand.Login(LoginDTO dto)
        {
            _logger.LogInformation("Logging in user with name: {Username}", dto.username);
            var result = new UserLoginMessage
            {
                UserName = dto.username,
                Status = "LoggingIn"
            };
            try
            {
                var user = await _userRepository.GetUserByUsername(dto.username);
                if (user == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("User does not exist");
                }
                var isValidPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, dto.password);
                if (!isValidPassword)
                {
                    result.StatusCode = 401;
                    throw new Exception("Password incorrect");
                }
                var token = _jwtService.GenerateToken(user.UserName, user.Id, user.role);

                var cachUser = _userStateModel.Create(user);
                await _daprClient.SaveStateAsync("blogstatestore", user.Id.ToString(), cachUser);

                result.Token = token;
                result.UserId = user.Id.ToString();
                result.Status = "LoggedIn";
                result.StatusCode = 200;
                return result;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error logging in user {UserName}", dto.username);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }


        async Task<UserResultMessage> IUserCommand.UpdateUser(Guid userId, UpdateUserDTO dto, Guid authId)
        {
            var result = new UserResultMessage
            {
                UserId = userId.ToString(),
                Status = "Updating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("User not found");
                }
                if (user.Id != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to update this user");
                }

                var hashedPassword = _passwordHasher.HashPassword(dto.Password);
                user.UpdateUser(dto.UserName, dto.Email, hashedPassword, dto.DisplayName);

                await _userRepository.UpdateUser(user, user.RowVersion);
                await _unitOfWork.CommitAsync();

                var userStateModel = _userStateModel.Create(user);
                await _daprClient.SaveStateAsync("blogstatestore", user.Id.ToString(), userStateModel);

                result.Status = "Updated";
                result.UserName = user.UserName;
                result.StatusCode = 200;
                
                _logger.LogInformation("User {UserId} updated successfully", userId);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

        async Task<UserResultMessage> IUserCommand.DeleteUser(Guid userId, Guid authId)
        {
            var result = new UserResultMessage
            {
                UserId = userId.ToString(),
                Status = "Deleting"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("User not found");
                }
                if (user.Id != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to delete this user");
                }

                await _userRepository.DeleteUser(user, user.RowVersion);
                await _daprClient.DeleteStateAsync("blogstatestore", userId.ToString());
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("User {UserId} deleted successfully", userId);
                result.Status = "Deleted";
                result.StatusCode = 200;
                return result;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }
    }

}
