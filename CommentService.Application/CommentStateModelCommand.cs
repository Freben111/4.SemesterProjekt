using CommentService.Application.Interfaces;
using CommentService.Domain;
using Shared.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application
{
    public class CommentStateModelCommand : ICommentStateModelCommand
    {
        public CommentStateModel Create(Comment comment)
        {
            return new CommentStateModel
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId
            };
        }
    }
}
