using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        /// <summary>
        /// Hello World example.
        /// </summary>
        Task HelloWorldAsync();

        /// <summary>
        /// Sends a message to this player.
        /// </summary>
        Task MessageAsync(Message message);

        /// <summary>
        /// Returns the latest received messages as cached by this player.
        /// </summary>
        Task<ImmutableList<Message>> GetMessagesAsync();
    }
}