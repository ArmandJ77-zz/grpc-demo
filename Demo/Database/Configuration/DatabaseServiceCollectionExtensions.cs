using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Configuration
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string dbConnectionString)
        {
            services
                .AddDbContext<DemoDbContext>(o => o.UseSqlServer(dbConnectionString));
            return services;
        }
    }
}
