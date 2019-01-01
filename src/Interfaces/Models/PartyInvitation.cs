using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class PartyInvitation : Message
    {
        public PartyInvitation(Guid id, DateTime timestamp, IPlayer player, IParty party)
            : base(id, timestamp)
        {
            Player = player;
            Party = party;
        }

        public IPlayer Player { get; }
        public IParty Party { get; }
    }
}
