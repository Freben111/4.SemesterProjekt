using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain
{
    public class User
    {
        public Guid Id { get; protected set; }
        public string UserName { get; protected set; }
        public string Email { get; protected set; }
        public string HashedPassword { get; protected set; }
        public string DisplayName { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public string role { get; protected set; } = "User";
        [Timestamp]
        public uint RowVersion { get; protected set; }


        public User()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public static User CreateUser(string userName, string email, string hashedPassword, string displayName)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = email,
                HashedPassword = hashedPassword,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        public void UpdateUser(string? userName, string? email, string? hashedPassword, string? displayName)
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(hashedPassword) && string.IsNullOrEmpty(displayName))
                throw new ArgumentException("At least one field must be provided for update.");
            this.UserName = userName ?? this.UserName;
            this.Email = email ?? this.Email;
            this.HashedPassword = hashedPassword ?? this.HashedPassword;
            this.DisplayName = displayName ?? this.DisplayName;
            this.UpdatedAt = DateTime.UtcNow;
        }



    }
}
