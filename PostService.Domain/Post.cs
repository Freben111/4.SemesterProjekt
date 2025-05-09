using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Domain
{
    public class Post
    {
        public Guid Id { get; protected set; }
        public string Title { get; protected set; }
        public string Content { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        [Timestamp]
        public uint RowVersion { get; protected set; }
        public Guid ForumId { get; protected set; }
        public Guid AuthorId { get; protected set; }



        public Post()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }



        public static Post CreatePost(string title, string content, string forumId, Guid authorId)
        {
            return new Post
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ForumId = Guid.Parse(forumId),
                AuthorId = authorId
            };
        }

        public void UpdatePost(string title, string content)
        {
            this.Title = title;
            this.Content = content;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
