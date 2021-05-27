using Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Functional.Helpers
{
    public static class DbContextExtensions
    {
        public static void Clear(this DemoDbContext db)
        {
            db.Items.Clear();
            db.SaveChanges();
        }

        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
