using Microsoft.Extensions.Options;
using Silo.Options;
using Silo.Services;

namespace UnitTests.Fakes
{
    public class FakeSupportApiOptions : IOptions<SupportApiOptions>
    {
        public SupportApiOptions Value { get; } = new SupportApiOptions
        {
            Title = nameof(SupportApiHostedService),
            Port = 0
        };
    }
}
