using Shared.Forum;
using Shared.Forum.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application.Interfaces
{
    public interface IForumCommand
    {

        Task<ForumResultMessage> CreateForum(CreateForumDTO dto);

        Task<ForumResultMessage> UpdateForum(Guid forumId, UpdateForumDTO dto);

        Task<ForumResultMessage> DeleteForum(Guid forumId);
    }
}
