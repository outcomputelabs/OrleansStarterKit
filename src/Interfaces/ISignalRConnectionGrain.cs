using Orleans;
using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISignalRConnectionGrain : IGrainWithGuidKey
    {
        Task SendMessageAsync(Guid channelId, string userName, string message);
    }
}
