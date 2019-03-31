using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IUserGrain : IGrainWithGuidKey
    {
        Task SetInfoAsync(UserInfo info);
        Task<UserInfo> GetInfoAsync();
        Task TellAsync(Message message);
        Task<ImmutableList<Message>> GetLatestMessagesAsync();
    }
}