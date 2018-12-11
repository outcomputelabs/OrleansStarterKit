using Grains.Models;
using Orleans;
using Orleans.Concurrency;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// The persisted state of a lobby.
    /// </summary>
    public class LobbyState
    {
        /// <summary>
        /// The list of users that this channel indexes.
        /// </summary>
        public SortedSet<UserInfo> Users { get; set; }
    }

    /// <inheritdoc />
    [Reentrant]
    public class Lobby : Grain<LobbyState>, ILobby
    {
        /// <inheritdoc />
        public override Task OnActivateAsync()
        {
            // initialize empty state
            if (State.Users == null) State.Users = new SortedSet<UserInfo>();

            return base.OnActivateAsync();
        }

        /// <inheritdoc />
        public Task<ImmutableList<UserInfo>> GetUserInfoListAsync()
        {
            return Task.FromResult(State.Users.ToImmutableList());
        }

        /// <inheritdoc />
        public Task SetUserInfoAsync(UserInfo info)
        {
            // add or update any existing listing information
            // if there are casing differences in the handle
            // then the new handle wins
            State.Users.Remove(info);
            State.Users.Add(info);
            return WriteStateAsync();
        }
    }
}
