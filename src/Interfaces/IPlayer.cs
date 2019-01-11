using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        Task TellOtherAsync(IPlayer other, PlayerMessage message);
        Task TellAsync(PlayerMessage message);

        Task<ImmutableList<Message>> GetMessagesAsync();

        Task InviteAsync(IPlayer target);
        Task ReceiveInviteAsync(PartyInvite invite);
    }
}