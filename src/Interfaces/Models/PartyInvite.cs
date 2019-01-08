using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class PartyInvite : Message
    {
        public PartyInvite(IPlayer from, IParty party)
        {
            From = from;
            Party = party;
        }

        public IPlayer From { get; }
        public IParty Party { get; }
    }
}
