using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        public Task Ping()
        {
            return Task.CompletedTask;
        }
    }
}
