using Dapr.Client;
using ForumService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Forum.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumService.Infrastructure.Database
{
    public class ForumQuery : IForumQuery
    {
        private readonly DaprClient _daprClient;
        private readonly ForumDbContext _db;

        public ForumQuery(ForumDbContext db, DaprClient daprClient)
        {
            _db = db;
            _daprClient = daprClient;
        }


        async Task<ForumDTO> IForumQuery.GetForum(Guid id)
        {
            var cachedForum = await _daprClient.GetStateAsync<ForumDTO>("blogstatestore", id.ToString());
            if (cachedForum != null)
            {
                return cachedForum;
            }
            var forum = await _db.Forums
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
            var forumDTO = new ForumDTO
            {
                Id = forum.Id,
                Name = forum.Name,
                RowVersion = forum.RowVersion
            };
            return forumDTO;
        }
        async Task<IEnumerable<ForumDTO>> IForumQuery.GetAllForums()
        {
            var forums = await _db.Forums
                .AsNoTracking()
                .ToListAsync();
            List<ForumDTO> forumDTOs = new List<ForumDTO>();
            foreach (var forum in forums)
            {
                var forumDTO = new ForumDTO
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Description = forum.Description,
                    RowVersion = forum.RowVersion
                };
                forumDTOs.Add(forumDTO);
            }
            return forumDTOs;
        }

    }
}
