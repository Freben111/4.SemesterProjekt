using ForumService.Domain;
using ForumService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Infrastructure.Database
{
    public class ForumRepository : IForumRepository
    {
        private readonly ForumDbContext _db;

        public ForumRepository(ForumDbContext db)
        {
            _db = db;
        }


        async Task IForumRepository.CreateForum(Forum forum)
        {
            if (_db.Forums.Any(f => f.Name == forum.Name))
            {
                throw new InvalidOperationException("Forum already exists.");
            }
            await _db.Forums.AddAsync(forum);
            await _db.SaveChangesAsync();
        }


        async Task IForumRepository.UpdateForum(Forum forum, uint rowVersion)
        {
            _db.Entry(forum).Property(nameof(forum.RowVersion)).OriginalValue = rowVersion;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The forum has been modified by another user.");
            }
        }
        async Task IForumRepository.DeleteForum(Forum forum, uint rowVersion)
        {
            _db.Entry(forum).Property(nameof(forum.RowVersion)).OriginalValue = rowVersion;
            _db.Forums.Remove(forum);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The forum has been modified by another user.");
            }
        }

        async Task<Forum> IForumRepository.GetForumById(Guid id)
        {
            var forum = await _db.Forums
                .FirstOrDefaultAsync(f => f.Id == id);
            if (forum == null)
            {
                throw new KeyNotFoundException("Forum not found.");
            }
            return forum;
        }
    }
}
