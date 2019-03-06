using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class TestGrain : Grain, ITestGrain
    {
        public Task<Guid> GetKeyAsync()
        {
            return Task.FromResult(this.GetPrimaryKey());
        }
    }
}
