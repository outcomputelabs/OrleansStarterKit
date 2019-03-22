using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class UserGrain : Grain<UserState>, IUserGrain
    {
        public Task SetInfoAsync(UserInfo info)
        {
            if (info.Key != GrainKey) throw new InvalidOperationException();

            State.Info = info;
            return WriteStateAsync();
        }

        private Guid GrainKey => this.GetPrimaryKey();
    }

    public class UserState
    {
        public UserInfo Info { get; set; }
    }
}
