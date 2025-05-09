using Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain;

namespace UserService.Application.Interfaces
{
    public interface IUserStateModelCommand
    {
        public UserStateModel Create(User user);
    }
}
