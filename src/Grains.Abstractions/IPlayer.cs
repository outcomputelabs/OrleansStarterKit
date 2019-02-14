using Grains.Models;
using Orleans;
using System;
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

        /// <summary>
        /// Makes this player invite another player to form a party.
        /// This player will become the initial leader of the party.
        /// </summary>
        Task<InviteResult> InviteAsync(IPlayer other);

        /// <summary>
        /// Invites this player to a party lead by another player.
        /// </summary>
        Task<InviteResult> TakeInviteAsync(Invite invite);
    }
}