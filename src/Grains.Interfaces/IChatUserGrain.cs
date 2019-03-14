using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IChatUserGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Sends a message to this user.
        /// </summary>
        Task MessageAsync(ChatMessage message);

        /// <summary>
        /// Returns the latest received messages as cached by this user.
        /// </summary>
        Task<ImmutableList<ChatMessage>> GetMessagesAsync();
    }
}