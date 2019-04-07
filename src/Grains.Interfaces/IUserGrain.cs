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

        /// <summary>
        /// Makes the user join the given channel.
        /// </summary>
        Task JoinChannelAsync(IChannelGrain channel);

        /// <summary>
        /// Returns the list of channels this user has joined.
        /// </summary>
        Task<ImmutableList<ChannelUser>> GetChannelsAsync();
    }
}