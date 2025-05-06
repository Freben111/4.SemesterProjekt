
using Shared.User;
using Shared.User.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces
{
    public interface IUserQuery
    {
        Task<UserDTO> GetUser(Guid Id);

        Task<IEnumerable<UserDTO>> GetAllUsers();
    }
}
