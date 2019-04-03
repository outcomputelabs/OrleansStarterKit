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

        /// <summary>
        /// Adds a user to the channel.
        /// This method is intended for calling from the <see cref="IUserGrain"/> info.
        /// Do not call this method from the client api.
        /// </summary>
        Task AddUserAsync(IUserGrain user);
    }
}
