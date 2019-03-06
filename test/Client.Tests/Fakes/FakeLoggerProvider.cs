using Microsoft.Extensions.Logging;

namespace Client.Tests.Fakes
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
