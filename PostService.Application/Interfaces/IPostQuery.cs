
using Shared.Post;
using Shared.Post.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces
{
    public interface IPostQuery
    {
        Task<PostDTO> GetPost(Guid Id);

        Task<IEnumerable<PostDTO>> GetAllPosts();
    }
}
