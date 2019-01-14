using Grains.Models;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IParty : IGrainWithGuidKey
    {
        Task SetLeaderAsync(IPlayer player);
        Task<IPlayer> GetLeaderAsync();

        Task<InviteResult> InviteAsync(IPlayer sender, IPlayer other);

        Task<bool> IsActiveAsync();
    }
}