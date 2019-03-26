using Orleans.Runtime.Services;
using System;

namespace Grains
{
    public class PersistenceServiceClient : GrainServiceClient<IPersistenceService>, IPersistenceServiceClient
    {
        public PersistenceServiceClient(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
