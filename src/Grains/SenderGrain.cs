using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class SenderGrain : Grain, ISenderGrain
    {
        public SenderGrain(ILogger<SenderGrain> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<SenderGrain> _logger;

        private Guid GrainKey => this.GetPrimaryKey();
        private string GrainType => nameof(SenderGrain);

        private IAsyncStream<StreamItem> _stream;
        private readonly Random _random = new Random();
        private readonly string[] _fields = new[] { "A", "B", "C" };

        public override Task OnActivateAsync()
        {
            _stream = GetStreamProvider("SMS").GetStream<StreamItem>(Guid.Empty, nameof(StreamItem));

            RegisterTimer(_ => SendAsync(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            return base.OnActivateAsync();
        }

        public Task StartAsync()
        {
            _logger.LogInformation("{@GrainType} {@GrainKey} started.", GrainType, GrainKey);
            return Task.CompletedTask;
        }

        private Task SendAsync()
        {
            var field = _fields[_random.Next(0, _fields.Length)];
            _logger.LogInformation("{@GrainType} {@GrainKey} sending item with field {@Field}",
                GrainType, GrainKey, field);

            return _stream.OnNextAsync(new StreamItem(field));
        }
    }
}
