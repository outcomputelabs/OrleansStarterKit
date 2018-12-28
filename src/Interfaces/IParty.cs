using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public interface IParty : IGrainWithGuidKey
    {
        Task SetInfoAsync(string description);
    }
}
