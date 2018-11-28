using Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public class Channel : Grain, IChannel
    {
        private string _name;


    }
}
