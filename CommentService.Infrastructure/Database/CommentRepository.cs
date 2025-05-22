using CommentService.Domain;
using CommentService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Infrastructure.Database
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentDbContext _db;

        public CommentRepository(CommentDbContext db)
        {
            _db = db;
        }


        async Task ICommentRepository.CreateComment(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
        }


        async Task ICommentRepository.UpdateComment(Comment comment, uint rowVersion)
        {
            _db.Entry(comment).Property(nameof(comment.RowVersion)).OriginalValue = rowVersion;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The comment has been modified by another user.");
            }
        }
        async Task ICommentRepository.DeleteComment(Comment comment, uint rowVersion)
        {
            _db.Entry(comment).Property(nameof(comment.RowVersion)).OriginalValue = rowVersion;
            _db.Comments.Remove(comment);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("The post has been modified by another user.");
            }
        }

        async Task<Comment> ICommentRepository.GetCommentById(Guid id)
        {
            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                throw new KeyNotFoundException("Post not found.");
            }
            return comment;
        }

        async Task<List<Comment>> ICommentRepository.GetCommentsByPostId(List<Guid> postIds)
        {
            var comments = await _db.Comments
                .Where(c => postIds.Contains(c.PostId.GetValueOrDefault()))
                .ToListAsync();
            if (comments.Count == 0)
            {
                throw new KeyNotFoundException("Comments not found.");
            }
            return comments;
        }
    }
}
