using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IParty : IGrainWithGuidKey
    {
        Task CreateAsync(IPlayer leader);
        Task InviteAsync(IPlayer sender, IPlayer invitee);
    }
}