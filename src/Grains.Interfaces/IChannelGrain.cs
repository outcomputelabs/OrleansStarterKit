using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChannelGrain : IGrainWithGuidKey
    {
        Task SetInfoAsync(ChannelInfo info);
        Task<ChannelInfo> GetInfoAsync();
        Task TellAsync(Message message);
        Task<ImmutableList<Message>> GetLatestMessagesAsync();
    }
}
