using Grains.Models;
using Orleans;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IPlayer : IGrainWithStringKey
    {
        Task SendTellAsync(TellMessage message);
        Task ReceiveTellAsync(TellMessage message);
        Task<ImmutableList<IMessage>> GetMessagesAsync();
    }
}