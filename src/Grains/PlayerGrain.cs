using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private string GrainKey => this.GetPrimaryKeyString();

        public async Task SetInfoAsync(PlayerInfo info)
        {
            // validate input consistency
            if (info.Handle != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // apply to grain state
            State.Info = info;
            await WriteStateAsync();

            // update the registry
            await GrainFactory.GetGrain<IPlayerRegistryGrain>(0).RegisterAsync(info);
        }

        public Task<PlayerInfo> GetInfoAsync() => Task.FromResult(State.Info);
    }

    public class PlayerState
    {
        public PlayerInfo Info { get; set; }
    }
}
