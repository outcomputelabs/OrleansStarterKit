using Grains.Models;
using Grains.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Grains
{
    [Reentrant]
    [StatelessWorker]
    public class SqlServerUserRegistryGrain : Grain, IUserRegistryGrain
    {
        private readonly SqlServerUserRegistryOptions _options;

        public SqlServerUserRegistryGrain(IOptions<SqlServerUserRegistryOptions> options)
        {
            _options = options.Value;
        }

        public async Task<UserInfo> GetAsync(Guid id)
        {
            using (var context = new SqlServerUserRegistryContext(_options))
            {
                return await context.Users.FindAsync(id);
            }
        }

        public async Task<UserInfo> GetByHandleAsync(string handle)
        {
            using (var context = new SqlServerUserRegistryContext(_options))
            {
                return await context.Users.SingleOrDefaultAsync(_ => _.Handle == handle);
            }
        }

        public async Task RegisterAsync(UserInfo entity)
        {
            using (var context = new SqlServerUserRegistryContext(_options))
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
            using (var context = new SqlServerUserRegistryContext(_options))
            {
                context.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

    public class SqlServerUserRegistryContext : DbContext
    {
        private readonly SqlServerUserRegistryOptions _options;

        public SqlServerUserRegistryContext(SqlServerUserRegistryOptions options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_options.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().HasKey(_ => _.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserInfo> Users { get; set; }
    }
}
