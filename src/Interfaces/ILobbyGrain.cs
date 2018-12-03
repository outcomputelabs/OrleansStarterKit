using Interfaces.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {
        Task<ImmutableList<Channel>> GetChannels();
        Task CreateChannel(string name);
        Task RemoveChannel(Guid id);
    }
}
