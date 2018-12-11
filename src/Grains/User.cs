using Grains.Models;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class UserState
    {
        public UserInfo UserInfo { get; set; }
    }

    [Reentrant]
    public class User : Grain<UserState>, IUser
    {
        private string _key;

        public override Task OnActivateAsync()
        {
            // only allow trimmed lower case grain keys
            _key = this.GetPrimaryKeyString();
            if (string.IsNullOrWhiteSpace(_key)) throw new InvalidUserGrainKeyException(_key);
            if (_key != _key.Trim().ToLowerInvariant()) throw new InvalidUserGrainKeyException(_key);

            return base.OnActivateAsync();
        }

        public Task<UserInfo> GetInfoAsync()
        {
            return Task.FromResult(State.UserInfo);
        }

        public async Task SetInfoAsync(UserInfo info)
        {
            // validate
            if (info == null) throw new ArgumentNullException(nameof(info));

            // safety check
            // username must be same as grain key
            // different casing is allowed for display purposes
            if (info.UserName.ToLowerInvariant() != _key) throw new InconsistentUserNameException(nameof(info.UserName));

            // all good so keep the new info
            State.UserInfo = info;

            // persist the info before attempting to register with the lobby
            await WriteStateAsync();

            // also register the new user info with the lobby
            await GrainFactory.GetGrain<ILobby>(Guid.Empty).SetUserInfoAsync(info);
        }
    }
}
