using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ILobby : IGrainWithGuidKey
    {
        Task<ImmutableDictionary<Guid, string>> GetChannels();
    }
}
