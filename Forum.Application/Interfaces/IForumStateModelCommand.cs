using ForumService.Domain;
using Shared.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application.Interfaces
{
    public interface IForumStateModelCommand
    {
        public ForumStateModel Create(Forum forum);
    }
}
