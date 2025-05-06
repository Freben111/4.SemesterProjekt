using Shared.User;
using Shared.User.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces
{
    public interface IUserCommand
    {

        Task<UserResultMessage> CreateUser(CreateUserDTO dto);

        Task<UserResultMessage> UpdateUser(Guid userId, UpdateUserDTO dto, Guid authId);

        Task<UserResultMessage> DeleteUser(Guid userId, Guid authId);
        Task<UserLoginMessage> Login(LoginDTO dto);
    }
}