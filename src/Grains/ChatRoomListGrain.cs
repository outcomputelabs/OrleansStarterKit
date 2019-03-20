using Grains.Models;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    public class ChatRoomListGrain : Grain<ChatRoomListState>, IChatRoomListGrain
    {
        public Task AddOrUpdateAsync(ChatRoomInfo info)
        {
            State.List.Remove(info);
            State.List.Add(info);

            return WriteStateAsync();
        }

        public override Task OnActivateAsync()
        {
            if (State.List == null) State.List = new HashSet<ChatRoomInfo>();

            return base.OnActivateAsync();
        }
    }

    public class ChatRoomListState
    {
        public HashSet<ChatRoomInfo> List { get; set; }
    }
}
