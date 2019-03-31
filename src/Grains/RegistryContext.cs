using Grains.Models;
using Microsoft.EntityFrameworkCore;

namespace Grains
{
    public class RegistryContext : DbContext
    {
        public RegistryContext(DbContextOptions<RegistryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>(e =>
            {
                e.HasKey(_ => _.Id);
                e.HasIndex(_ => _.Handle).IsUnique();
                e.Property(_ => _.Name);
            });

            modelBuilder.Entity<ChannelInfo>(e =>
            {
                e.HasKey(_ => _.Id);
                e.HasIndex(_ => _.Handle).IsUnique();
                e.Property(_ => _.Description);
            });

            modelBuilder.Entity<Message>(e =>
            {
                e.HasKey(_ => _.Id);
                e.HasIndex(_ => new { _.SenderId, _.Timestamp });
                e.HasIndex(_ => new { _.ReceiverId, _.Timestamp });
                e.Property(_ => _.SenderHandle);
                e.Property(_ => _.SenderName);
                e.Property(_ => _.Content);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserInfo> Users { get; set; }
        public DbSet<ChannelInfo> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
