using Interfaces;
using Interfaces.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class LobbyGrain : Grain, ILobbyGrain
    {
        private ChannelCollection _channels = new ChannelCollection();

        public LobbyGrain()
        {
            // add fake data
            _channels.Add(new ChannelInfo(Guid.NewGuid(), "Channel 1"));
            _channels.Add(new ChannelInfo(Guid.NewGuid(), "Channel 2"));
            _channels.Add(new ChannelInfo(Guid.NewGuid(), "Channel 3"));
        }

        public Task<ImmutableList<ChannelInfo>> GetChannels()
        {
            return Task.FromResult(_channels.ToImmutableList());
        }

        public Task CreateChannel(string name)
        {
            _channels.Add(new ChannelInfo(Guid.NewGuid(), name));
            return Task.CompletedTask;
        }

        public Task RemoveChannel(Guid id)
        {
            _channels.Remove(id);
            return Task.CompletedTask;
        }
    }
}
