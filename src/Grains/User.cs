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
        public Task<UserInfo> GetInfoAsync()
        {
            return Task.FromResult(State.UserInfo);
        }

        public override Task OnActivateAsync()
        {
            // initialize empty state
            if (State.UserInfo == null)
            {
                State.UserInfo = new UserInfo(this.GetPrimaryKeyString(), string.Empty);
            }

            return base.OnActivateAsync();
        }

        public Task UpdateInfoAsync(UserInfo info)
        {
            if (info.UserName != State.UserInfo.UserName)
            {
                throw new ArgumentException(nameof(UserInfo.UserName));
            }

            State.UserInfo = info;

            return WriteStateAsync();
        }
    }
}
