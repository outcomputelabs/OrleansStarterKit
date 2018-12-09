using Grains.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class ChannelState
    {
        public ChannelInfo Info { get; set; }
    }

    public class Channel : Grain<ChannelState>, IChannel
    {
        private string _key;

        public override Task OnActivateAsync()
        {
            _key = this.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        public Task SetInfoAsync(ChannelInfo info)
        {
            // info must be defined
            if (info == null) throw new ArgumentNullException(nameof(info));

            // name must be defined
            if (string.IsNullOrWhiteSpace(info.Name)) throw new ArgumentNullException(nameof(info.Name));

            // name must be valid
            if (info.Name != info.Name.Trim().ToLowerInvariant()) throw new InvalidChannelNameException(info.Name);

            // name must be same as key
            if (info.Name != _key) throw new InvalidChannelNameException(info.Name);

            // all good so keep the info
            State.Info = info;

            // done
            return WriteStateAsync();
        }
    }
}
