using UserService.Domain;
using UserService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _db;

        public UserRepository(UserDbContext db)
        {
            _db = db;
        }


        async Task IUserRepository.CreateUser(User user)
        {
            if (_db.Users.Any(u => u.UserName == user.UserName))
            {
                throw new InvalidOperationException("UserName already in use");
            }
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }


        async Task IUserRepository.UpdateUser(User user, uint rowVersion)
        {
            _db.Entry(user).Property(nameof(user.RowVersion)).OriginalValue = rowVersion;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The user has been modified by another user.");
            }
        }
        async Task IUserRepository.DeleteUser(User user, uint rowVersion)
        {
            _db.Entry(user).Property(nameof(user.RowVersion)).OriginalValue = rowVersion;
            _db.Users.Remove(user);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The user has been modified by another user.");
            }
        }

        async Task<User> IUserRepository.GetUserById(Guid id)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return user;
        }

        async Task<User> IUserRepository.GetUserByUsername(string username)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return user;
        }
    }
}
