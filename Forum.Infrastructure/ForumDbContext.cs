using ForumService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ForumService.Infrastructure
{
    public class ForumDbContext : DbContext
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options) { }

        public DbSet<Forum> Forums { get; set; }
    }
}
