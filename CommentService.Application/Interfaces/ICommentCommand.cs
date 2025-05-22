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

        Task<CommentResultMessage> CreateComment(CreateCommentDTO dto, Guid authId);

        Task<CommentResultMessage> UpdateComment(Guid forumId, UpdateCommentDTO dto, Guid authId);

        Task<CommentResultMessage> DeleteComment(Guid forumId, Guid authId);

        Task<CommentResultMessage> DeleteCommentByPostIds(List<Guid> postIds, Guid authId);
    }
}
