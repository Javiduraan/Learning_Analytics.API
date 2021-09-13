using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class UsedContext : DbContext
    {
        public UsedContext(DbContextOptions<UsedContext> options) : base(options)
        {
        
        }

        public DbSet<Users> Users { get; set; }
    }
}