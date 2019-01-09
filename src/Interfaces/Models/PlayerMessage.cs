using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class PlayerMessage : Message
    {
        public PlayerMessage(IPlayer from, IPlayer to, string content)
        {
            From = from;
            To = to;
            Content = content;
        }

        public IPlayer From { get; }
        public IPlayer To { get; }
        public string Content { get; }
    }
}
