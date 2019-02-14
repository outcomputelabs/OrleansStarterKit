using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Grains
{
    public class ReceiverGrain : Grain, IReceiverGrain, IAsyncObserver<StreamItem>
    {
        public ReceiverGrain(ILogger<ReceiverGrain> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<ReceiverGrain> _logger;
        private IAsyncStream<StreamItem> _stream;
        private StreamSubscriptionHandle<StreamItem> _handle;

        private readonly string _grainType = nameof(ReceiverGrain);
        private string _grainKey;

        public override async Task OnActivateAsync()
        {
            _grainKey = this.GetPrimaryKeyString();
            _stream = GetStreamProvider("SMS").GetStream<StreamItem>(Guid.Empty, nameof(StreamItem));

            // recover subscription
            var handles = await _stream.GetAllSubscriptionHandles();
            var first = await handles.FirstOrDefault()?.ResumeAsync(this);

            // remove any duplicate subs
            await Task.WhenAll(handles.Skip(1).Select(x => x.UnsubscribeAsync()));

            await base.OnActivateAsync();
        }

        public async Task StartAsync()
        {
            // unsubscribe from all
            await Task.WhenAll((await _stream.GetAllSubscriptionHandles()).Select(x => x.UnsubscribeAsync()));

            // sub to new one
            await _stream.SubscribeAsync(this, null, FilterStreamItem, _grainKey);
        }

        public static bool FilterStreamItem(IStreamIdentity stream, object filterData, object item) => ((StreamItem)item).Field == filterData.ToString();

        public Task OnNextAsync(StreamItem item, StreamSequenceToken token = null)
        {
            _logger.LogInformation("{@GrainType} {@GrainKey} received item with field {@Field}",
                _grainType, _grainKey, item.Field);

            return Task.CompletedTask;
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _logger.LogError(ex, "{@GrainType} {@GrainKey} received stream error {@Message}", ex.Message);
            return Task.CompletedTask;
        }
    }
}
