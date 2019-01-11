using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class PartyMessage : Message
    {
        public PartyMessage(IPlayer from, string content)
        {
            From = from;
            Content = content;
        }

        public IPlayer From { get; }
        public string Content { get; }
    }
}
