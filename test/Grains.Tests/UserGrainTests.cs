using Grains.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;
using System.Linq;
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
            var state = await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).GetUserAsync(key);
            Assert.Equal(info, state);
        }

        [Fact]
        public async Task GetInfoAsync_Gets_State()
        {
            // arrange
            var key = Guid.NewGuid();
            var info = new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).RegisterUserAsync(info);
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);

            // act
            var state = await grain.GetInfoAsync();

            // assert
            Assert.Equal(info, state);
        }

        [Fact]
        public async Task TellAsync_Saves_State()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            var message = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            await grain.SetInfoAsync(new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            // act
            await grain.TellAsync(message);

            // assert
            var state = await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).GetMessageAsync(message.Id);
            Assert.Equal(message, state);
        }

        [Fact]
        public async Task GetLatestMessagesAsync_Returns_Tells()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            await grain.SetInfoAsync(new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            var message1 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message2 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message3 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);

            // act
            await grain.TellAsync(message1);
            await grain.TellAsync(message2);
            await grain.TellAsync(message3);
            var state = await grain.GetLatestMessagesAsync();

            // assert
            Assert.Collection(state,
                m => Assert.Equal(message1, m),
                m => Assert.Equal(message2, m),
                m => Assert.Equal(message3, m));
        }

        [Fact]
        public async Task GetLatestMessagesAsync_Returns_State()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            var message1 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message2 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            var message3 = new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), key, Guid.NewGuid().ToString(), DateTime.Now);
            await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).RegisterMessageAsync(message1);
            await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).RegisterMessageAsync(message2);
            await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).RegisterMessageAsync(message3);

            // act
            var state = await grain.GetLatestMessagesAsync();

            // assert
            Assert.Collection(state,
                m => Assert.Equal(message1, m),
                m => Assert.Equal(message2, m),
                m => Assert.Equal(message3, m));
        }

        [Fact]
        public async Task JoinChannelAsync_Saves_User_State()
        {
            // arrange
            var userKey = Guid.NewGuid();
            var user = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(userKey);
            var channel1 = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(Guid.NewGuid());
            var channel2 = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(Guid.NewGuid());
            var channel3 = _fixture.Cluster.GrainFactory.GetGrain<IChannelGrain>(Guid.NewGuid());

            // act
            await user.JoinChannelAsync(channel1);
            await user.JoinChannelAsync(channel2);
            await user.JoinChannelAsync(channel3);

            // assert
            var state = await _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty).GetChannelsByUserAsync(userKey);
            Assert.Collection(state,
                m => Assert.Equal(channel1.GetPrimaryKey(), m.ChannelId),
                m => Assert.Equal(channel2.GetPrimaryKey(), m.ChannelId),
                m => Assert.Equal(channel3.GetPrimaryKey(), m.ChannelId));
        }

        [Fact]
        public async Task GetChannelsAsync_Returns_User_State()
        {
            // arrange
            var userKey = Guid.NewGuid();
            var user = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(userKey);
            var storage = _fixture.Cluster.GrainFactory.GetGrain<IStorageRegistryGrain>(Guid.Empty);
            var channels = new ChannelUser[]
            {
                new ChannelUser(Guid.NewGuid(), userKey),
                new ChannelUser(Guid.NewGuid(), userKey),
                new ChannelUser(Guid.NewGuid(), userKey)
            };
            foreach (var channel in channels)
            {
                await storage.RegisterChannelUserAsync(channel);
            }

            // act
            var state = await user.GetChannelsAsync();

            // assert
            Assert.Collection(state,
                m => Assert.Equal(channels[0].ChannelId, m.ChannelId),
                m => Assert.Equal(channels[1].ChannelId, m.ChannelId),
                m => Assert.Equal(channels[2].ChannelId, m.ChannelId));
        }

        [Fact]
        public async Task ValidateMessage_Refuses_Null_Messages()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            await grain.SetInfoAsync(new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            // act and assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await grain.TellAsync(null);
            });
        }

        [Fact]
        public async Task ValidateMessage_Refuses_Mismatched_Key()
        {
            // arrange
            var key = Guid.NewGuid();
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IUserGrain>(key);
            await grain.SetInfoAsync(new UserInfo(key, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            // act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await grain.TellAsync(new Message(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid(), Guid.NewGuid().ToString(), DateTime.Now));
            });
        }
    }
}
