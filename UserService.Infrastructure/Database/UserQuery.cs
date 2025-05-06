using Dapr.Client;
using UserService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.User.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Database
{
    public class UserQuery : IUserQuery
    {
        private readonly DaprClient _daprClient;
        private readonly UserDbContext _db;

        public UserQuery(UserDbContext db, DaprClient daprClient)
        {
            _db = db;
            _daprClient = daprClient;
        }


        async Task<UserDTO> IUserQuery.GetUser(Guid id)
        {
            var cachedUser = await _daprClient.GetStateAsync<UserDTO>("statestore", id.ToString());
            if (cachedUser != null)
            {
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
            return userDTO;
        }
        async Task<IEnumerable<UserDTO>> IUserQuery.GetAllUsers()
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
            return userDTOs;
        }

    }
}
