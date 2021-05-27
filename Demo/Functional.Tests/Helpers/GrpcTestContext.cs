using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Tests.Functional.Helpers
{
    internal class GrpcTestContext<TStartup> : IDisposable where TStartup : class
    {
        private readonly ExecutionContext _executionContext;
        private readonly Stopwatch _stopwatch;
        private readonly GrpcTestFixture<TStartup> _fixture;

        public GrpcTestContext(GrpcTestFixture<TStartup> fixture)
        {
            _executionContext = ExecutionContext.Capture()!;
            _stopwatch = Stopwatch.StartNew();
            _fixture = fixture;
            _fixture.LoggedMessage += WriteMessage;
        }

        private void WriteMessage(LogLevel logLevel, string category, EventId eventId, string message, Exception exception)
        {
            // Log using the passed in execution context.
            // In the case of NUnit, console output is only captured by the test
            // if it is written in the test's execution context.
            ExecutionContext.Run(_executionContext, s =>
            {
                Console.WriteLine($"{_stopwatch.Elapsed.TotalSeconds:N3}s {category} - {logLevel}: {message}");
            }, null);
        }

        public void Dispose()
        {
            _fixture.LoggedMessage -= WriteMessage;
            _executionContext?.Dispose();
        }
    }
}
