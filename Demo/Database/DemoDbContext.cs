using Database.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class DemoDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options)
        {
        }
    }
}
