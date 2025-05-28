using Dapr.Client;
using UserService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.User.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserService.Infrastructure.Database
{
    public class UserQuery : IUserQuery
    {
        private readonly ILogger<UserQuery> _logger;
        private readonly DaprClient _daprClient;
        private readonly UserDbContext _db;

        public UserQuery(ILogger<UserQuery> logger, UserDbContext db, DaprClient daprClient)
        {
            _logger = logger;
            _db = db;
            _daprClient = daprClient;
        }


        async Task<UserDTO> IUserQuery.GetUser(Guid id)
        {
            _logger.LogInformation("Getting user with id: {Id}", id);
            try
            {
                var cachedUser = await _daprClient.GetStateAsync<UserDTO>("blogstatestore", id.ToString());
                if (cachedUser != null)
                {
                    _logger.LogInformation("User with id: {Id} found in cache", id);
                    return cachedUser;
                }
                var user = await _db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
                var userDTO = new UserDTO
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Password = user.HashedPassword,
                    DisplayName = user.DisplayName,
                    RowVersion = user.RowVersion
                };
                if (userDTO != null)
                {
                    _logger.LogInformation("User with id: {Id} found in database", id);
                    await _daprClient.SaveStateAsync("statestore", id.ToString(), userDTO);
                }
                else
                {
                    _logger.LogWarning("User with id: {Id} not found in database", id);
                    throw new Exception("User not found");
                }
                return userDTO;
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving user", ex);
            }
        }
        async Task<IEnumerable<UserDTO>> IUserQuery.GetAllUsers()
        {
            _logger.LogInformation("Getting all users");
            try
            {
                var users = await _db.Users
                    .AsNoTracking()
                    .ToListAsync();
                List<UserDTO> userDTOs = new List<UserDTO>();
                foreach (var user in users)
                {
                    var userDTO = new UserDTO
                    {
                        Id = user.Id.ToString(),
                        UserName = user.UserName,
                        Password = user.HashedPassword,
                        DisplayName = user.DisplayName,
                        RowVersion = user.RowVersion
                    };
                    userDTOs.Add(userDTO);
                }
                if (userDTOs.Count <= 0)
                {
                    _logger.LogWarning("No users found in database");
                    throw new Exception("No users found");

                }
                return userDTOs;
            }
            catch (Exception ex) 
            {
                throw new Exception("Error retrieving all users", ex);
            }
        }

    }
}
