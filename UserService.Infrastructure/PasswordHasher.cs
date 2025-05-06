
using Isopoh.Cryptography.Argon2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        string IPasswordHasher.HashPassword(string password)
        {
            return Argon2.Hash(password);
        }
        bool IPasswordHasher.VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return Argon2.Verify(hashedPassword, providedPassword);
        }
    }
}
