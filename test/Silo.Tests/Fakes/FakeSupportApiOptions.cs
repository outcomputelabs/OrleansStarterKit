using Microsoft.Extensions.Options;
using Silo.Options;

namespace Silo.Tests.Fakes
{
    public class FakeSupportApiOptions : IOptions<SupportApiOptions>
    {
        public SupportApiOptions Value { get; set; } = new SupportApiOptions
        {
            Title = nameof(SupportApiHostedService)
        };
    }
}
