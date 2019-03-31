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
    /// <summary>
    /// Implements a <see cref="IMessageRegistryGrain"/> that relies on entity framework.
    /// </summary>
    [Reentrant]
    [StatelessWorker]
    public class EntityFrameworkMessageRegistryGrain : Grain, IMessageRegistryGrain
    {
        private readonly Func<RegistryContext> _factory;

        public EntityFrameworkMessageRegistryGrain(Func<RegistryContext> factory)
        {
            _factory = factory;
        }

        public async Task<Message> GetAsync(Guid id)
        {
            return await _factory().Messages.FindAsync(id);
        }

        public async Task<ImmutableList<Message>> GetLatestByReceiverIdAsync(Guid receiverId, int count)
        {
            var result = await _factory().Messages
                .Where(_ => _.ReceiverId == receiverId)
                .OrderByDescending(_ => _.Timestamp)
                .Take(count)
                .ToListAsync();

            return result.ToImmutableList();
        }

        public async Task RegisterAsync(Message entity)
        {
            var context = _factory();
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

        public async Task UnregisterAsync(Message entity)
        {
            var context = _factory();
            context.Messages.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
