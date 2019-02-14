using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class StreamItem
    {
        public StreamItem(string field)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
