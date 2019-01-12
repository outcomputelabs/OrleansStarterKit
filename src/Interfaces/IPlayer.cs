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
        /// Sends a direct message to another player.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task TellAsync(Message message);

        Task<ImmutableList<Message>> GetMessagesAsync();
    }
}