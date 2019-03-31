using Grains.Models;
using Microsoft.EntityFrameworkCore;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Implements a <see cref="IChannelRegistryGrain"/> that relies on entity framework.
    /// </summary>
    [Reentrant]
    [StatelessWorker]
    public class EntityFrameworkChannelRegistryGrain : Grain, IChannelRegistryGrain
    {
        private readonly Func<RegistryContext> _factory;

        public EntityFrameworkChannelRegistryGrain(Func<RegistryContext> factory)
        {
            _factory = factory;
        }

        public Task<ChannelInfo> GetAsync(Guid id)
        {
            return _factory()
                .Channels
                .FindAsync(id);
        }

        public Task<ChannelInfo> GetByHandleAsync(string handle)
        {
            return _factory()
                .Channels
                .SingleOrDefaultAsync(_ => _.Handle == handle);
        }

        public async Task RegisterAsync(ChannelInfo entity)
        {
            var context = _factory();
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

        public async Task UnregisterAsync(ChannelInfo entity)
        {
            var context = _factory();
            context.Channels.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
