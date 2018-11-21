using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUser: IGrainWithGuidKey
    {
        Task Ping();
    }
}
