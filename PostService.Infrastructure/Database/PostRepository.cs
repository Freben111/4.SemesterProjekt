using PostService.Domain;
using PostService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Infrastructure.Database
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _db;

        public PostRepository(PostDbContext db)
        {
            _db = db;
        }


        async Task IPostRepository.CreatePost(Post post)
        {
            if (_db.Posts.Any(p => p.Title == post.Title))
            {
                throw new InvalidOperationException("Post already exists.");
            }
            await _db.Posts.AddAsync(post);
            await _db.SaveChangesAsync();
        }


        async Task IPostRepository.UpdatePost(Post post, uint rowVersion)
        {
            _db.Entry(post).Property(nameof(post.RowVersion)).OriginalValue = rowVersion;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The post has been modified by another user.");
            }
        }
        async Task IPostRepository.DeletePost(Post post, uint rowVersion)
        {
            _db.Entry(post).Property(nameof(post.RowVersion)).OriginalValue = rowVersion;
            _db.Posts.Remove(post);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The post has been modified by another user.");
            }
        }

        async Task<Post> IPostRepository.GetPostById(Guid id)
        {
            var post = await _db.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found.");
            }
            return post;
        }

        async Task<List<Post>> IPostRepository.GetPostsByForumId(Guid forumId)
        {
            var posts = await _db.Posts
                .Where(p => p.ForumId == forumId)
                .ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                return new List<Post>();
            }
            return posts;
        }
    }
}
