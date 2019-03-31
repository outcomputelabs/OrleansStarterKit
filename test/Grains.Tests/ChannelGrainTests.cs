using Grains.Models;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class ChannelGrainTests
    {
        private readonly ClusterFixture _fixture;

        public ChannelGrainTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SetInfoAsync_Saves_State()
        {
            // arrange
            var id = Guid.NewGuid();
            var handle = "SomeHandle";
            var description = "SomeDescription";
            var info = new ChannelInfo(id, handle, description);

            // act
            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .SetInfoAsync(info);

            // assert the state was saved
            var state = await _fixture.SiloServiceProvider.GetService<RegistryContext>()
                .Channels
                .FindAsync(id);

            Assert.Equal(info.Id, state.Id);
            Assert.Equal(info.Handle, state.Handle);
            Assert.Equal(info.Description, state.Description);
        }

        [Fact]
        public async Task SetInfoAsync_Caches_State()
        {
            // arrange
            var id = Guid.NewGuid();
            var handle = "SomeHandle";
            var description = "SomeDescription";
            var info = new ChannelInfo(id, handle, description);

            // act
            await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .SetInfoAsync(info);

            // assert the state is available
            var state = await _fixture.Cluster.GrainFactory
                .GetGrain<IChannelGrain>(id)
                .GetInfoAsync();

            Assert.Equal(info.Id, state.Id);
            Assert.Equal(info.Handle, state.Handle);
            Assert.Equal(info.Description, state.Description);
        }
    }
}
