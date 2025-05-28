using Dapr.Client;
using PostService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Post.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Infrastructure.Database
{
    public class PostQuery : IPostQuery
    {
        private readonly DaprClient _daprClient;
        private readonly PostDbContext _db;

        public PostQuery(PostDbContext db, DaprClient daprClient)
        {
            _db = db;
            _daprClient = daprClient;
        }


        async Task<PostDTO> IPostQuery.GetPost(Guid id)
        {
            var cachedPost = await _daprClient.GetStateAsync<PostDTO>("blogstatestore", id.ToString());
            if (cachedPost != null)
            {
                return cachedPost;
            }
            var post = await _db.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            var postDTO = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ForumId = post.ForumId,
                AuthorId = post.AuthorId,
                RowVersion = post.RowVersion
            };
            return postDTO;
        }
        async Task<IEnumerable<PostDTO>> IPostQuery.GetAllPosts()
        {
            var posts = await _db.Posts
                .AsNoTracking()
                .ToListAsync();
            List<PostDTO> postDTOs = new List<PostDTO>();
            foreach (var post in posts)
            {
                var postDTO = new PostDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    ForumId = post.ForumId,
                    AuthorId = post.AuthorId,
                    RowVersion = post.RowVersion
                };
                postDTOs.Add(postDTO);
            }
            return postDTOs;
        }

    }
}
