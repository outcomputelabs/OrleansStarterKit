using Grains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orleans.Configuration;
using Orleans.Hosting;
using System;

namespace Silo
{
    public static class SiloHostBuilderExtensions
    {
        public static ISiloHostBuilder TryUseLocalhostClustering(this ISiloHostBuilder builder, IConfiguration configuration, int siloPort, int gatewayPort)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Orleans:Providers:Clustering:Provider"] == "Localhost")
            {
                builder.UseLocalhostClustering(siloPort, gatewayPort, null,
                        configuration["Orleans:ServiceId"],
                        configuration["Orleans:ClusterId"]);
            }
            return builder;
        }

        public static ISiloHostBuilder TryUseAdoNetClustering(this ISiloHostBuilder builder, IConfiguration configuration, int siloPort, int gatewayPort)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var orleans = configuration.GetSection("Orleans");
            var section = orleans.GetSection("Providers:Clustering");

            if (section["Provider"] == "AdoNet")
            {
                section = section.GetSection("AdoNet");

                builder
                    .ConfigureEndpoints(siloPort, gatewayPort)
                    .UseAdoNetClustering(_ =>
                    {
                        _.ConnectionString = configuration.GetConnectionString(section["ConnectionStringName"]);
                        _.Invariant = section["Invariant"];
                    })
                    .Configure<ClusterOptions>(_ =>
                    {
                        _.ClusterId = orleans["ClusterId"];
                        _.ServiceId = orleans["ServiceId"];
                    });
            }

            return builder;
        }

        public static ISiloHostBuilder TryUseInMemoryReminderService(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Orleans:Providers:Reminders:Provider"] == "InMemory")
            {
                builder.UseInMemoryReminderService();
            }

            return builder;
        }

        public static ISiloHostBuilder TryUseAdoNetReminderService(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Orleans:Providers:Reminders");
            if (section["Provider"] == "AdoNet")
            {
                section = section.GetSection("AdoNet");
                builder.UseAdoNetReminderService(_ =>
                {
                    _.ConnectionString = configuration.GetConnectionString(
                        section["ConnectionStringName"]);

                    _.Invariant = section["Invariant"];
                });
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddMemoryGrainStorageAsDefault(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Orleans:Providers:Storage:Default:Provider"] == "InMemory")
            {
                builder.AddMemoryGrainStorageAsDefault();
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddAdoNetGrainStorageAsDefault(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Orleans:Providers:Storage:Default");
            if (section["Provider"] == "AdoNet")
            {
                section = section.GetSection("AdoNet");
                builder.AddAdoNetGrainStorageAsDefault(_ =>
                {
                    _.ConnectionString = configuration.GetConnectionString(section["ConnectionStringName"]);
                    _.Invariant = section["Invariant"];
                    _.UseJsonFormat = section.GetValue<bool>("UseJsonFormat");
                    _.TypeNameHandling = section.GetValue<TypeNameHandling>("TypeNameHandling");
                });
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddMemoryGrainStorageForPubSub(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Orleans:Providers:Storage:PubSub:Provider"] == "InMemory")
            {
                builder.AddMemoryGrainStorage("PubSubStore");
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddSqlServerRegistry(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Registry:Provider"] == "SqlServer")
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<RegistryContext>(options =>
                    {
                        options.UseSqlServer(
                            configuration.GetConnectionString(configuration["Registry:Providers:SqlServer:ConnectionStringName"]));
                    }, ServiceLifetime.Transient);
                    services.AddSingleton<Func<RegistryContext>>(_ => () => _.GetService<RegistryContext>());
                });
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddInMemoryRegistry(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Registry:Provider"] == "InMemory")
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<RegistryContext>(options =>
                    {
                        options.UseInMemoryDatabase(
                            configuration["Registry:Providers:InMemory:DatabaseName"]);
                    }, ServiceLifetime.Transient);
                    services.AddSingleton<Func<RegistryContext>>(_ => () => _.GetService<RegistryContext>());
                });
            }

            return builder;
        }

        public static ISiloHostBuilder TryAddAdoNetGrainStorageForPubSub(this ISiloHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Orleans:Providers:Storage:PubSub");
            if (section["Provider"] == "AdoNet")
            {
                section = section.GetSection("AdoNet");

                builder.AddAdoNetGrainStorage("PubSubStore", _ =>
                {
                    _.ConnectionString = configuration.GetConnectionString(section["ConnectionStringName"]);
                    _.Invariant = section["Invariant"];
                    _.UseJsonFormat = section.GetValue<bool>("UseJsonFormat");
                });
            }

            return builder;
        }
    }
}
