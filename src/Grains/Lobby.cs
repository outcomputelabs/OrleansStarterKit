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
        /// Ths list of accounts that this lobby tracks.
        /// </summary>
        public Dictionary<string, AccountInfo> Accounts;
    }

    /// <inheritdoc />
    [Reentrant]
    public class Lobby : Grain<LobbyState>, ILobby
    {
        /// <inheritdoc />
        public override Task OnActivateAsync()
        {
            // initialize empty state
            if (State.Accounts == null) State.Accounts = new Dictionary<string, AccountInfo>();

            return base.OnActivateAsync();
        }

        /// <inheritdoc />
        public Task<ImmutableList<AccountInfo>> GetUserInfoListAsync()
        {
            return Task.FromResult(State.Accounts.Values.ToImmutableList());
        }

        /// <inheritdoc />
        public Task SetUserInfoAsync(AccountInfo info)
        {
            State.Accounts[info.UniformHandle] = info;

            return WriteStateAsync();
        }
    }
}
