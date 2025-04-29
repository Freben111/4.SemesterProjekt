using Shared.Comment;
using Shared.Comment.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Interfaces
{
    public interface ICommentCommand
    {

        Task<CommentResultMessage> CreateComment(CreateCommentDTO dto);

        Task<CommentResultMessage> UpdateComment(Guid forumId, UpdateCommentDTO dto);

        Task<CommentResultMessage> DeleteComment(Guid forumId);
    }
}
