using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Client.Console.Tests.Fakes
{
    public class FakeClusterClient : IClusterClient
    {
        public bool IsInitialized => throw new NotSupportedException();

        public IServiceProvider ServiceProvider => throw new NotSupportedException();

        public Task AbortAsync()
        {
            throw new NotSupportedException();
        }

        public void BindGrainReference(IAddressable grain)
        {
            throw new NotSupportedException();
        }

        public Task Close()
        {
            throw new NotSupportedException();
        }

        public Task Connect(Func<Exception, Task<bool>> retryFilter = null)
        {
            throw new NotSupportedException();
        }

        public Task<TGrainObserverInterface> CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            throw new NotSupportedException();
        }

        public Task DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj) where TGrainObserverInterface : IGrainObserver
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            throw new NotSupportedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidKey
        {
            throw new NotSupportedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerKey
        {
            throw new NotSupportedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithStringKey
        {
            throw new NotSupportedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidCompoundKey
        {
            throw new NotSupportedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            throw new NotSupportedException();
        }

        public IStreamProvider GetStreamProvider(string name)
        {
            throw new NotSupportedException();
        }
    }
}
