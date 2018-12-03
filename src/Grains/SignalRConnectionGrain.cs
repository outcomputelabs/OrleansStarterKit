using System;
using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class SignalRConnectionGrain : Grain, ISignalRConnectionGrain
    {
        public Task SendMessageAsync(Guid channelId, string userName, string message)
        {
            return Task.CompletedTask;
        }
    }
}
