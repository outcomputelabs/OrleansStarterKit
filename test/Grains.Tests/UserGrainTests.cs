using Grains.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class UserGrainTests
    {
        private readonly ClusterFixture _fixture;

        public UserGrainTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SetInfoAsync_Saves_State()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            var info = new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // act
            await grain.SetInfoAsync(info);

            // assert
            var context = _fixture.SiloServiceProvider.GetService<RegistryContext>();
            var state = await context.Users.FindAsync(key);
            Assert.Equal(info, state);
        }
    }
}
