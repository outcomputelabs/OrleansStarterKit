using Microsoft.Extensions.Options;
using Silo.Options;

namespace UnitTests.Fakes
{
    public class FakeSiloHostedServiceOptions : IOptions<SiloHostedServiceOptions>
    {
        public SiloHostedServiceOptions Value { get; set; } = new SiloHostedServiceOptions();
    }
}
