using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Domain
{
    public class Comment
    {
        public Guid Id { get; protected set; }
        public string Content { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        [Timestamp]
        public uint RowVersion { get; protected set; }
        public Guid? PostId { get; protected set; }

        public Guid? UserId { get; protected set; }



        public Comment()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }



        public static Comment CreateComment(string content, string postId, string userId)
        {
            return new Comment
            {
                Id = Guid.NewGuid(),
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PostId = Guid.Parse(postId),
                UserId = Guid.Parse(userId)
            };
        }

        public void UpdateComment(string content)
        {
            this.Content = content;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
