using ForumService.Application.Interfaces;
using ForumService.Domain;
using Shared.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Application
{
    public class ForumStateModelCommand : IForumStateModelCommand
    {
        ForumStateModel IForumStateModelCommand.Create(Forum forum)
        {
            return new ForumStateModel
            {
                Id = forum.Id,
                Name = forum.Name,
                Description = forum.Description,
                RowVersion = forum.RowVersion,
                OwnerId = forum.OwnerId,
                ModeratorIds = forum.ModeratorIds
            };
        }
    }
}
