using Forum.Domain;
using Microsoft.EntityFrameworkCore;

namespace ForumService.Infrastructure.Database
{
    public class ForumDbContext : DbContext
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options) { }

        public DbSet<Forum.Domain.Forum> Forums { get; set; } = null!;
    }
}
