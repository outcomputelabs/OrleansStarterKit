using Orleans.Concurrency;
using Orleans.Runtime;

namespace Grains
{
    [Reentrant]
    public class ChatCacheGrainService : GrainService, IChatCacheGrainService
    {
    }
}
