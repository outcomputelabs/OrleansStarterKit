using Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public class ChannelGrain : Grain, IChannelGrain
    {
        private string _name;


    }
}
