using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <inheritdoc />
    public class ChannelRegistryGrain : Grain<ChannelRegistryState>, IChatRoomRegistryGrain
    {
        /// <inheritdoc />
        public Task<bool> ExistsAsync() => Task.FromResult(State.Key != Guid.Empty);

        /// <inheritdoc />
        public async Task<IChatRoomGrain> GetOrCreateChatRoomAsync()
        {
            if (State.Key == Guid.Empty)
            {
                State.Key = Guid.NewGuid();
                await WriteStateAsync();
            }
            return GrainFactory.GetGrain<IChatRoomGrain>(State.Key);
        }
    }

    public class ChannelRegistryState
    {
        public Guid Key { get; set; }
    }
}
