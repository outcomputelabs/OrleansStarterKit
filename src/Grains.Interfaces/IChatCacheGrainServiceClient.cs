using Orleans.Services;

namespace Grains
{
    public interface IChatCacheGrainServiceClient : IGrainServiceClient<IChatCacheGrainService>, IChatCacheGrainService
    {
    }
}
