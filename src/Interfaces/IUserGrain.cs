using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        Task Ping();
    }
}
