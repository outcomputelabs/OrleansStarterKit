using System;
using System.Threading.Tasks;

namespace Grains
{
    public class Party : IParty
    {
        public Task SetInfoAsync(string description)
        {
            throw new NotImplementedException();
        }
    }
}