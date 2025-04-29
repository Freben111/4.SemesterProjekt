using Dapr.Client;
using CommentService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Comment.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Infrastructure.Database
{
    public class CommentQuery : ICommentQuery
    {
        private readonly DaprClient _daprClient;
        private readonly CommentDbContext _db;

        public CommentQuery(CommentDbContext db, DaprClient daprClient)
        {
            _db = db;
            _daprClient = daprClient;
        }


        async Task<CommentDTO> ICommentQuery.GetComment(Guid id)
        {
            var cachedComment = await _daprClient.GetStateAsync<CommentDTO>("statestore", id.ToString());
            if (cachedComment != null)
            {
                return cachedComment;
            }
            var comment = await _db.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            var commentDTO = new CommentDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                RowVersion = comment.RowVersion
            };
            return commentDTO;
        }
        async Task<IEnumerable<CommentDTO>> ICommentQuery.GetAllComments()
        {
            var comments = await _db.Comments
                .AsNoTracking()
                .ToListAsync();
            List<CommentDTO> commentDTOs = new List<CommentDTO>();
            foreach (var comment in comments)
            {
                var commentDTO = new CommentDTO
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    RowVersion = comment.RowVersion
                };
                commentDTOs.Add(commentDTO);
            }
            return commentDTOs;
        }

    }
}
