using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        Task CreatePartyAsync(string description);
    }
}
