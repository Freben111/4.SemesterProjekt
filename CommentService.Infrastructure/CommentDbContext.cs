using CommentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
    }
}
