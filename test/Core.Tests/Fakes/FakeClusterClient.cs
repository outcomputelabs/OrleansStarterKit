using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tests.Fakes
{
    public class FakeClusterClient : IClusterClient
    {
        public bool IsInitialized => throw new NotImplementedException();

        public IServiceProvider ServiceProvider => throw new NotImplementedException();

        public Task AbortAsync()
        {
            throw new NotImplementedException();
        }

        public void BindGrainReference(IAddressable grain)
        {
            throw new NotImplementedException();
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task Connect(Func<Exception, Task<bool>> retryFilter = null)
        {
            throw new NotImplementedException();
        }

        public Task<TGrainObserverInterface> CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            throw new NotImplementedException();
        }

        public Task DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidKey
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerKey
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithStringKey
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidCompoundKey
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            throw new NotImplementedException();
        }

        public IStreamProvider GetStreamProvider(string name)
        {
            throw new NotImplementedException();
        }
    }
}
