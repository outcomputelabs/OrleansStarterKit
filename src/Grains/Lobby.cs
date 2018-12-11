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
        public ImmutableSortedSet<UserInfo> Users { get; set; } = ImmutableSortedSet<UserInfo>.Empty;
    }

    /// <inheritdoc />
    [Reentrant]
    public class Lobby : Grain<LobbyState>, ILobby
    {
        /// <inheritdoc />
        public Task<IEnumerable<UserInfo>> GetUserInfoListAsync()
        {
            return Task.FromResult<IEnumerable<UserInfo>>(State.Users);
        }

        /// <inheritdoc />
        public Task SetUserInfoAsync(UserInfo info)
        {
            // add or update any existing listing information
            // if there are casing differences in the handle
            // then the new handle wins
            State.Users = State.Users.Remove(info).Add(info);
            return WriteStateAsync();
        }
    }
}
