using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class Invite
    {
        public Invite(IParty party, IPlayer from)
        {
            Party = party;
            From = from;
        }

        public IParty Party { get; }
        public IPlayer From { get; }
    }
}
