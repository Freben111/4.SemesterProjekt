using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userName, Guid userId, string role);
    }
}
