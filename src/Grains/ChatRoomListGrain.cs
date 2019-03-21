using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.EventSourcing;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    [Reentrant]
    public class ChatRoomListGrain : JournaledGrain<ChatRoomListState>, IChatRoomListGrain
    {
        private readonly ILogger<ChatRoomListGrain> _logger;

        private TaskCompletionSource<ChatRoomListPollResult> _future;
        private Task<ChatRoomListPollResult> _present;

        public ChatRoomListGrain(ILogger<ChatRoomListGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            // initialize state if needed
            if (State.List == null) State.List = new HashSet<ChatRoomInfo>();

            // initialize reactive promises
            _future = new TaskCompletionSource<ChatRoomListPollResult>();
            _present = Task.FromResult(new ChatRoomListPollResult(Version, State.List.ToImmutableList()));

            return base.OnActivateAsync();
        }

        public async Task<int> SetAsync(ChatRoomInfo info)
        {
            RaiseEvent(new ChatRoomSetEvent(info));
            await ConfirmEvents();

            return Version;
        }

        public Task<ImmutableList<ChatRoomInfo>> GetAsync() => Task.FromResult(State.List.ToImmutableList());

        public Task<ChatRoomListPollResult> PollAsync(int version) => version == Version ? _future.Task : _present;

        protected override void TransitionState(ChatRoomListState state, object @event)
        {
            switch (@event)
            {
                case ChatRoomSetEvent set:
                    State.List.Remove(set.Info);
                    State.List.Add(set.Info);
                    break;
            }

            base.TransitionState(state, @event);
        }

        protected override void OnStateChanged()
        {
            // compute the new read-to-send result
            var result = new ChatRoomListPollResult(Version, State.List.ToImmutableList());

            // fulfill the current reactive promise and create a new one
            _future.SetResult(result);
            _future = new TaskCompletionSource<ChatRoomListPollResult>();

            // ready the data for sending for present requests
            _present = Task.FromResult(result);

            base.OnStateChanged();
        }
    }

    public class ChatRoomListState
    {
        public HashSet<ChatRoomInfo> List { get; set; }
    }

    [Immutable]
    public class ChatRoomSetEvent
    {
        public ChatRoomSetEvent(ChatRoomInfo info)
        {
            Info = info;
        }

        public ChatRoomInfo Info { get; }
    }
}