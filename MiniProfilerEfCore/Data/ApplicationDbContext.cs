using Microsoft.EntityFrameworkCore;

namespace MiniProfilerEfCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
