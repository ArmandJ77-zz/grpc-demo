using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;

namespace Tests.Functional.Helpers
{
    public delegate void LogMessage(LogLevel logLevel, 
        string categoryName, 
        EventId eventId, 
        string message, 
        Exception exception);

    public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;

        public readonly IHost Host;

        public event LogMessage? LoggedMessage;

        public LoggerFactory LoggerFactory { get; }

        public HttpMessageHandler Handler { get; }

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
                    }

                    services.AddDbContext<DemoDbContext>();
                })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost
                        .UseTestServer()
                        .UseStartup<TStartup>();
                });

            Host = builder.Start();
            
            _server = Host.GetTestServer();

            Handler = _server.CreateHandler();
        }


        public void Dispose()
        {
            Handler.Dispose();
            Host.Dispose();
            _server.Dispose();
        }

        public IDisposable GetTestContext()
        {
            return new GrpcTestContext<TStartup>(this);
        }
    }
}
