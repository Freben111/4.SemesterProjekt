using CommentService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Infrastructure.Security
{
    public class JwtValidator : IJwtValidator
    {
        private readonly TokenValidationParameters _params;

        public JwtValidator(IConfiguration config)
        {
            _params = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key"]))
            };
        }

        ClaimsPrincipal? IJwtValidator.ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, _params, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
