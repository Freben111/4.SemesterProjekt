
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        string IPasswordHasher.HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            
            var config = new Argon2Config
            {
                Type = Argon2Type.HybridAddressing,
                Password = passwordBytes,
                Salt = salt,
            };
            var argon2id = new Argon2(config);

            string hashString;
            using (SecureArray<byte> hashA = argon2id.Hash())
            {
                hashString = config.EncodeString(hashA.Buffer);
            }

            return hashString;

        }
        bool IPasswordHasher.VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return Argon2.Verify(hashedPassword, providedPassword);
        }
    }
}
