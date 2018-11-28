using Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class Lobby : Grain, ILobby
    {
        private Dictionary<Guid, string> _channels = new Dictionary<Guid, string>();

        public Task<ImmutableDictionary<Guid, string>> GetChannels()
        {
            return Task.FromResult(_channels.ToImmutableDictionary());
        }
    }
}
