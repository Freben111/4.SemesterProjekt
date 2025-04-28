
using Shared.Forum;
using Shared.Forum.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application.Interfaces
{
    public interface IForumQuery
    {
        Task<ForumDTO> GetForum(Guid Id);

        Task<IEnumerable<ForumDTO>> GetAllForums();
    }
}
