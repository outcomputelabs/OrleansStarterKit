using Grains.Models;
using Microsoft.EntityFrameworkCore;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Grains
{
    [Reentrant]
    [StatelessWorker]
    public class StorageRegistryGrain : Grain, IStorageRegistryGrain
    {
        private readonly Func<RegistryContext> _factory;

        public StorageRegistryGrain(Func<RegistryContext> factory)
        {
            _factory = factory;
        }

        #region Users

        public async Task<UserInfo> GetUserAsync(Guid id) => await _factory().Users.FindAsync(id);

        public async Task<UserInfo> GetUserByHandleAsync(string handle) => await _factory().Users.SingleOrDefaultAsync(_ => _.Handle == handle);

        public async Task RegisterUserAsync(UserInfo entity)
        {
            using (var context = _factory())
            {
                if (await context.Users.CountAsync(_ => _.Id == entity.Id) == 0)
                {
                    context.Users.Add(entity);
                }
                else
                {
                    context.Users.Update(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task UnregisterUserAsync(UserInfo entity)
        {
            using (var context = _factory())
            {
                context.Users.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region Channels

        public Task<ChannelInfo> GetChannelAsync(Guid id)
        {
            using (var context = _factory())
            {
                return context.Channels
                    .FindAsync(id);
            }
        }

        public Task<ChannelInfo> GetChannelByHandleAsync(string handle)
        {
            using (var context = _factory())
            {
                return context.Channels
                    .SingleOrDefaultAsync(_ => _.Handle == handle);
            }
        }

        public async Task RegisterChannelAsync(ChannelInfo entity)
        {
            using (var context = _factory())
            {
                if (await context.Channels.CountAsync(_ => _.Id == entity.Id) == 0)
                {
                    context.Channels.Add(entity);
                }
                else
                {
                    context.Channels.Update(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task UnregisterChannelAsync(ChannelInfo entity)
        {
            using (var context = _factory())
            {
                context.Channels.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region Channel Users

        public async Task RegisterChannelUserAsync(ChannelUser entity)
        {
            using (var context = _factory())
            {
                if (await context.ChannelUsers.ContainsAsync(entity))
                {
                    context.ChannelUsers.Update(entity);
                }
                else
                {
                    context.ChannelUsers.Add(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task UnregisterChannelUserAsync(ChannelUser entity)
        {
            using (var context = _factory())
            {
                context.ChannelUsers.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<ImmutableList<ChannelUser>> GetUsersByChannelAsync(Guid channelId)
        {
            using (var context = _factory())
            {
                var result = await context.ChannelUsers.Where(_ => _.ChannelId == channelId).ToListAsync();
                return result.ToImmutableList();
            }
        }

        public async Task<ImmutableList<ChannelUser>> GetChannelsByUserAsync(Guid userId)
        {
            using (var context = _factory())
            {
                var result = await context.ChannelUsers.Where(_ => _.UserId == userId).ToListAsync();
                return result.ToImmutableList();
            }
        }

        #endregion

        #region Messages

        public async Task<ImmutableList<Message>> GetLatestMessagesByReceiverIdAsync(Guid receiverId, int count)
        {
            using (var context = _factory())
            {
                var result = await context.Messages
                    .Where(_ => _.ReceiverId == receiverId)
                    .OrderByDescending(_ => _.Timestamp)
                    .Take(count)
                    .ToListAsync();

                return result.ToImmutableList();
            }
        }

        public async Task<Message> GetMessageAsync(Guid id)
        {
            using (var context = _factory())
            {
                return await context.Messages
                    .FindAsync(id);
            }
        }

        public async Task RegisterMessageAsync(Message entity)
        {
            using (var context = _factory())
            {
                if (await context.Messages.FindAsync(entity.Id) == null)
                {
                    context.Messages.Add(entity);
                }
                else
                {
                    context.Messages.Update(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task UnregisterMessageAsync(Message entity)
        {
            using (var context = _factory())
            {
                context.Messages.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
