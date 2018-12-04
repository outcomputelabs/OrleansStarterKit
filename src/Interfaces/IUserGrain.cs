using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUserGrain: IGrainWithStringKey
    {
        Task Ping();
    }
}
