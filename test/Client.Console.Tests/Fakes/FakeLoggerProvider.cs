using Microsoft.Extensions.Logging;

namespace Client.Console.Tests.Fakes
{
    public class FakeLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FakeLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
