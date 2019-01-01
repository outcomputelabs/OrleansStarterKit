using Orleans.Concurrency;

namespace Grains.Models
{
    [Immutable]
    public class PartyInvitation : Message
    {
        public PartyInvitation(IPlayer player, IParty party)
        {
            Player = player;
            Party = party;
        }

        public IPlayer Player { get; }
        public IParty Party { get; }
    }
}
