using Orleans.Runtime.Services;
using System;

namespace Grains
{
    public class StorageGrainServiceClient : GrainServiceClient<IStorageGrainService>, IStorageGrainServiceClient
    {
        public StorageGrainServiceClient(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
