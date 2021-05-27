using System;
using System.Linq;
using System.Net.Http;
using Database;
using Database.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tests.Functional.Helpers
{
    public delegate void LogMessage(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception exception);

    public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;
        private readonly IHost _host;

        public event LogMessage? LoggedMessage;

        public GrpcTestFixture() : this(null) { }

        public GrpcTestFixture(Action<IServiceCollection>? initialConfigureServices)
        {
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddProvider(new ForwardingLoggerProvider((logLevel, category, eventId, message, exception) =>
            {
                LoggedMessage?.Invoke(logLevel, category, eventId, message, exception);
            }));

            var builder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    initialConfigureServices?.Invoke(services);
                    services.AddSingleton<ILoggerFactory>(LoggerFactory);

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<DemoDbContext>));

                    services.Remove(descriptor);

                    services.AddDbContext<DemoDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<DemoDbContext>();

                        db.Database.EnsureCreated();

                        //try
                        //{
                        //    Utilities.InitializeDbForTests(db);
                        //}
                        //catch (Exception ex)
                        //{
                        //    logger.LogError(ex, "An error occurred seeding the " +
                        //                        "database with test messages. Error: {Message}", ex.Message);
                        //}
                    }
                    
                    services.AddDbContext<DemoDbContext>();
                })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost
                        .UseTestServer()
                        .UseStartup<TStartup>();
                });
            _host = builder.Start();
            _server = _host.GetTestServer();

            Handler = _server.CreateHandler();
        }

        public LoggerFactory LoggerFactory { get; }

        public HttpMessageHandler Handler { get; }

        public void Dispose()
        {
            Handler.Dispose();
            _host.Dispose();
            _server.Dispose();
        }

        public IDisposable GetTestContext()
        {
            return new GrpcTestContext<TStartup>(this);
        }
    }
}
