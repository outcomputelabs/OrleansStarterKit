using Grains.Models;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    public class UserState
    {
        public AccountInfo AccountInfo { get; set; }

        public Dictionary<string, Tuple<AccountInfo, IAccount>> Following { get; set; }

        public Dictionary<string, Tuple<AccountInfo, IAccount>> Followers { get; set; }
    }

    [Reentrant]
    public class Account : Grain<UserState>, IAccount
    {
        private string _key;

        public override Task OnActivateAsync()
        {
            // only allow trimmed lower case grain keys
            _key = this.GetPrimaryKeyString();
            if (string.IsNullOrWhiteSpace(_key)) throw new InvalidGrainKeyException(_key);
            if (_key != _key.Trim().ToLowerInvariant()) throw new InvalidGrainKeyException(_key);

            // initialize empty state
            if (State.Following == null) State.Following = new Dictionary<string, Tuple<AccountInfo, IAccount>>();
            if (State.Followers == null) State.Followers = new Dictionary<string, Tuple<AccountInfo, IAccount>>();

            return base.OnActivateAsync();
        }

        public Task<AccountInfo> GetInfoAsync()
        {
            if (State.AccountInfo == null)
            {
                throw new InvalidOperationException();
            }

            return Task.FromResult(State.AccountInfo);
        }

        public async Task SetInfoAsync(AccountInfo info)
        {
            // validate
            if (info == null) throw new ArgumentNullException(nameof(info));

            // call consistency check
            // the uniform handle must be the same as the grain key
            if (info.UniformHandle != _key) throw new InvalidHandleException(nameof(info.UniformHandle));

            // all good so keep the new info
            State.AccountInfo = info;

            // persist the new account info before attempting to register with the lobby
            await WriteStateAsync();

            // register the new account with the lobby
            await GrainFactory.GetGrain<ILobby>(Guid.Empty).SetAccountInfoAsync(info);
        }

        public async Task FollowAsync(AccountInfo info, IAccount target)
        {
            // tell the target account that this account will follow it
            await target.FollowedByAsync(State.AccountInfo, this.AsReference<IAccount>());

            // add the account to the list of this account is following
            State.Following[info.UniformHandle] = Tuple.Create(info, target);

            await WriteStateAsync();
        }

        public Task FollowedByAsync(AccountInfo info, IAccount follower)
        {
            // add the given account as a follower
            State.Followers[info.UniformHandle] = Tuple.Create(info, follower);

            return WriteStateAsync();
        }

        public Task<ImmutableList<AccountInfo>> GetFollowingAsync()
        {
            return Task.FromResult(State.Following.Values.Select(x => x.Item1).ToImmutableList());
        }
    }
}
