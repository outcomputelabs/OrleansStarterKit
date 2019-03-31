using Grains.Models;
using Microsoft.EntityFrameworkCore;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// Implements a <see cref="IUserRegistryGrain"/> that relies on entity framework.
    /// </summary>
    [Reentrant]
    [StatelessWorker]
    public class EntityFrameworkUserRegistryGrain : Grain, IUserRegistryGrain
    {
        private readonly Func<RegistryContext> _factory;

        public EntityFrameworkUserRegistryGrain(Func<RegistryContext> factory)
        {
            _factory = factory;
        }

        public async Task<UserInfo> GetAsync(Guid id)
        {
            using (var context = _factory())
            {
                return await context.Users.FindAsync(id);
            }
        }

        public async Task<UserInfo> GetByHandleAsync(string handle)
        {
            using (var context = _factory())
            {
                return await context.Users.SingleOrDefaultAsync(_ => _.Handle == handle);
            }
        }

        public async Task RegisterAsync(UserInfo entity)
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

        public async Task UnregisterAsync(UserInfo entity)
        {
            using (var context = _factory())
            {
                context.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
