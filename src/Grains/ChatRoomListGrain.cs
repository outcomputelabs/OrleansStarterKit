using Grains.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class ChatRoomListGrain : Grain<ChatRoomListState>, IChatRoomListGrain
    {
        public async Task<Guid> AddOrUpdateAsync(ChatRoomInfo info)
        {
            State.List.Remove(info);
            State.List.Add(info);
            State.CacheToken = Guid.NewGuid();

            await WriteStateAsync();

            return State.CacheToken;
        }

        public Task<ImmutableList<ChatRoomInfo>> GetAsync() => Task.FromResult(State.List.ToImmutableList());

        public override Task OnActivateAsync()
        {
            if (State.List == null) State.List = new HashSet<ChatRoomInfo>();

            return base.OnActivateAsync();
        }
    }

    public class ChatRoomListState
    {
        public Guid CacheToken { get; set; }
        public HashSet<ChatRoomInfo> List { get; set; }
    }
}
