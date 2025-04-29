
using Shared.Comment;
using Shared.Comment.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Interfaces
{
    public interface ICommentQuery
    {
        Task<CommentDTO> GetComment(Guid Id);

        Task<IEnumerable<CommentDTO>> GetAllComments();
    }
}
