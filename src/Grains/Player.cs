using System;
using System.Threading.Tasks;
using Orleans;

namespace Grains
{
    public class Player : Grain, IPlayer
    {
        private IParty currentParty;

        public async Task CreatePartyAsync(string description)
        {
            if (currentParty != null)
                throw new InvalidOperationException();

            var partyKey = Guid.NewGuid();
            //var party = await GrainFactory.GetGrain<IParty>(partyKey).SetInfoAsync(description);
        }
    }
}
