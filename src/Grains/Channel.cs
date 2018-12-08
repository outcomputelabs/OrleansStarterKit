using Orleans;

namespace Grains
{
    public class ChannelState
    {
        public bool Created { get; set; } = false;
    }

    public class Channel : Grain<ChannelState>, IChannel
    {
    }
}
