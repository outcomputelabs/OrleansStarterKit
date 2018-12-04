using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class UserGrain : Grain, IUser
    {
        public Task Ping()
        {
            return Task.CompletedTask;
        }
    }
}
