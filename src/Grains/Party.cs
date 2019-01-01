using System;
using System.Threading.Tasks;

namespace Grains
{
    public class Party : IParty
    {
        private IPlayer _leader;

        public Task CreateAsync(IPlayer leader)
        {
            if (_leader == null)
            {
                _leader = leader;
                return Task.CompletedTask;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}