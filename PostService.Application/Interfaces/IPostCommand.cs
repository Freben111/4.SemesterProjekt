using Shared.Post;
using Shared.Post.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces
{
    public interface IPostCommand
    {

        Task<PostResultMessage> CreatePost(CreatePostDTO dto);

        Task<PostResultMessage> UpdatePost(Guid forumId, UpdatePostDTO dto);

        Task<PostResultMessage> DeletePost(Guid forumId);
    }
}
