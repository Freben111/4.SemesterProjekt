using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces
{
    public interface IJwtValidator
    {
        ClaimsPrincipal? ValidateToken(string token);
    }
}
