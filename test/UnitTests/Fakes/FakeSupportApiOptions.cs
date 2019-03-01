using Microsoft.Extensions.Options;
using Silo;
using Silo.Options;

namespace UnitTests.Fakes
{
    public class FakeSupportApiOptions : IOptions<SupportApiOptions>
    {
        public SupportApiOptions Value { get; set; } = new SupportApiOptions
        {
            Title = nameof(SupportApiHostedService)
        };
    }
}
