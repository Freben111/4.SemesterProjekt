using PostService.Application.Interfaces;
using PostService.Domain;
using Shared.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Application
{
    public class PostStateModelCommand : IPostStateModelCommand
    {
        public PostStateModel Create(Post post)
        {
            return new PostStateModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ForumId = post.ForumId,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId
            };
        }
    }
}
