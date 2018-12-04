using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUser: IGrainWithStringKey
    {
        Task Ping();
    }
}
