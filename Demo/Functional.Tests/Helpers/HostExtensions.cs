using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Tests.Functional.Helpers
{
    public static class HostExtensions
    {
        public static IHost Seed<TDbContext>(
            this IHost host,
            Action<TDbContext> seed)
            where TDbContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<TDbContext>();

            seed(service);
            service?.SaveChanges();
            return host;
        }
    }
}
