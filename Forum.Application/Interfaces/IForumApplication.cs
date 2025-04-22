using ForumService.Application.ServiceDTO;
using Shared.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application.Interfaces
{
    public interface IForumApplication
    {

        Task<ForumResultMessage> CreateForum(CreateForumDTO dto);
    }
}
