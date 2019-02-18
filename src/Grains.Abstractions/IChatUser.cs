using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChatUser : IGrainWithStringKey
    {
        /// <summary>
        /// Sends a message to this user.
        /// </summary>
        Task MessageAsync(Message message);

        /// <summary>
        /// Returns the latest received messages as cached by this user.
        /// </summary>
        Task<ImmutableList<Message>> GetMessagesAsync();
    }
}