using Orleans.Services;

namespace Grains
{
    public interface IStorageGrainServiceClient : IGrainServiceClient<IStorageGrainService>, IStorageGrainService
    {
    }
}
