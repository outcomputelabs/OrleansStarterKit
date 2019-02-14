using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class Invite
    {
        public Invite(IParty party, IPlayer from, IPlayer to, DateTime expiry)
        {
            Party = party;
            From = from;
            To = to;
            Expiry = expiry;
        }

        public IParty Party { get; }
        public IPlayer From { get; }
        public IPlayer To { get; }
        public DateTime Expiry { get; }
    }
}
