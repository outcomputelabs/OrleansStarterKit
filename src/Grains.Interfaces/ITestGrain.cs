using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// This is a test grain interface to assist connectivity tests.
    /// </summary>
    public interface ITestGrain : IGrainWithGuidKey
    {
        Task<Guid> GetKeyAsync();
    }
}
