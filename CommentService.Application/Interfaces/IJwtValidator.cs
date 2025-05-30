﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Interfaces
{
    public interface IJwtValidator
    {
        ClaimsPrincipal? ValidateToken(string token);
    }
}
