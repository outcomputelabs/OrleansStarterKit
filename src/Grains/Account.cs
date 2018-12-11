using Grains.Models;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class UserState
    {
        public AccountInfo UserInfo { get; set; }
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

            return base.OnActivateAsync();
        }

        public Task<AccountInfo> GetInfoAsync()
        {
            return Task.FromResult(State.UserInfo);
        }

        public async Task SetInfoAsync(AccountInfo info)
        {
            // validate
            if (info == null) throw new ArgumentNullException(nameof(info));

            // call consistency check
            // the handle must be the same as the grain key
            // different casing is allowed for display purposes
            if (info.Handle.ToLowerInvariant() != _key) throw new InvalidHandleException(nameof(info.Handle));

            // all good so keep the new info
            State.UserInfo = info;

            // persist the new account info before attempting to register with the lobby
            await WriteStateAsync();

            // register the new account with the lobby
            await GrainFactory.GetGrain<ILobby>(Guid.Empty).SetUserInfoAsync(info);
        }
    }
}
