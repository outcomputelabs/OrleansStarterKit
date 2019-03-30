using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IUserGrain : IGrainWithGuidKey
    {
        Task SetInfoAsync(UserInfo info);
        Task<UserInfo> GetInfoAsync();
        Task TellAsync(TellMessage message);
    }
}