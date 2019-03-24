using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        public override Task OnActivateAsync()
        {
            if (State.Info != null && State.Info.Handle != GrainKey)
            {
                throw new InvalidOperationException();
            }

            return base.OnActivateAsync();
        }

        private string GrainKey => this.GetPrimaryKeyString();

        public Task SetInfoAsync(PlayerInfo info)
        {
            if (info.Handle != GrainKey)
            {
                throw new InvalidOperationException();
            }

            State.Info = info;
            return WriteStateAsync();
        }

        public Task<PlayerInfo> GetInfoAsync() => Task.FromResult(State.Info);
    }

    public class PlayerState
    {
        public PlayerInfo Info { get; set; }
    }
}
