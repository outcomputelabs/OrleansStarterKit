using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <inheritdoc />
    public class ChannelRegistry : Grain<ChannelRegistryState>, IChannelRegistry
    {
        /// <inheritdoc />
        public async Task<Guid> GetOrCreateKeyAsync()
        {
            if (State.Key == Guid.Empty)
            {
                State.Key = Guid.NewGuid();
                await WriteStateAsync();
            }
            return State.Key;
        }
    }

    public class ChannelRegistryState
    {
        public Guid Key { get; set; }
    }
}
