using Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;
using UserService.Domain;

namespace UserService.Application
{
    public class UserStateModelCommand : IUserStateModelCommand
    {

        UserStateModel IUserStateModelCommand.Create(User user)
        {
            return new UserStateModel
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                RowVersion = user.RowVersion
            };
        }
    }
}
