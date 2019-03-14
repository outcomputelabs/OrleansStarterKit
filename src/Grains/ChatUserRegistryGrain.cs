using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <inheritdoc />
    public class ChatUserRegistryGrain : Grain<ChatUserRegistryState>, IChatUserRegistryGrain
    {
        /// <inheritdoc />
        public Task<bool> ExistsAsync() => Task.FromResult(State.Key != Guid.Empty);

        /// <inheritdoc />
        public async Task<IChatUserGrain> GetOrCreateChatRoomAsync()
        {
            if (State.Key == Guid.Empty)
            {
                State.Key = Guid.NewGuid();
                await WriteStateAsync();
            }
            return GrainFactory.GetGrain<IChatUserGrain>(State.Key);
        }
    }

    public class ChatUserRegistryState
    {
        public Guid Key { get; set; }
    }
}
