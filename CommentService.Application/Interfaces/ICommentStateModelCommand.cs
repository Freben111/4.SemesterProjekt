using CommentService.Domain;
using Shared.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Interfaces
{
    public interface ICommentStateModelCommand
    {
        public CommentStateModel Create(Comment comment);
    }
}
