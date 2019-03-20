using Orleans.Runtime.Services;
using System;

namespace Grains
{
    public class ChatCacheGrainServiceClient : GrainServiceClient<IChatCacheGrainService>, IChatCacheGrainServiceClient
    {
        public ChatCacheGrainServiceClient(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
