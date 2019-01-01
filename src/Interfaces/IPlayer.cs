using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        Task SendTellAsync(PlayerMessage message);
        Task ReceiveTellAsync(PlayerMessage message);
        Task<ImmutableList<Message>> GetMessagesAsync();
        Task InviteAsync(string target);

    }
}