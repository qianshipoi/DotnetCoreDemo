using LoggingGlobal.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace LoggingGlobal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<HttpLog> HttpLogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
