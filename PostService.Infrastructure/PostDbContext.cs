using PostService.Domain;
using Microsoft.EntityFrameworkCore;

namespace PostService.Infrastructure
{
    public class PostDbContext : DbContext
    {
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
    }
}
