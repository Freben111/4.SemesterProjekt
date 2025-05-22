using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Domain
{
    public class Forum
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        [Timestamp]
        public uint RowVersion { get; protected set; }
        public Guid OwnerId { get; protected set; }
        public List<Guid>? ModeratorIds { get; protected set; }



        public Forum()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }



        public static Forum CreateForum(string name, string description, Guid userId)
        {
            return new Forum
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OwnerId = userId,
            };
        }

        public void UpdateForum(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public static Forum RestoreForum(Guid id, string name, string description, DateTime createdAt, DateTime updatedAt, Guid ownerId, List<Guid> moderatorIds)
        {
            return new Forum
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                OwnerId = ownerId,
                ModeratorIds = moderatorIds
            };
        }
    }
}
