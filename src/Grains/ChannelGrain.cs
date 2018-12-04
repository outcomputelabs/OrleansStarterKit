using Interfaces;
using Orleans;

namespace Grains
{
    public class ChannelGrain : Grain<ChannelGrainState>, IChannelGrain
    {

    }

    public class ChannelGrainState
    {
    }
}
