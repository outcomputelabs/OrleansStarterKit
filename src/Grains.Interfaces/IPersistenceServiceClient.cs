using Orleans.Services;

namespace Grains
{
    public interface IPersistenceServiceClient : IGrainServiceClient<IPersistenceService>, IPersistenceService
    {
    }
}
