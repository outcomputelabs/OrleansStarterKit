using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class PlayerInfo
    {
        public PlayerInfo(string handle, string name)
        {
            Handle = handle;
            Name = name;
        }

        public string Handle { get; }
        public string Name { get; }
    }
}
