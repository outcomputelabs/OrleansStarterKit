using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        Task TellAsync(PlayerMessage message);
        Task ReceiveTellAsync(PlayerMessage message);

        Task<ImmutableList<Message>> GetMessagesAsync();

        Task InviteAsync(string target);
        Task ReceiveInviteAsync(PartyInvite invite);
    }
}