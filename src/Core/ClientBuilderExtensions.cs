using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;

namespace Core
{
    public static class ClientBuilderExtensions
    {
        public static IClientBuilder TryUseLocalhostClustering(this IClientBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (configuration["Orleans:Providers:Clustering:Provider"] == "Localhost")
            {
                builder.UseLocalhostClustering(
                    configuration.GetValue<int>("Orleans:Ports:Gateway:Start"),
                    configuration["Orleans:ServiceId"],
                    configuration["Orleans:ClusterId"]);
            }
            return builder;
        }

        public static IClientBuilder TryUseAdoNetClustering(this IClientBuilder builder, IConfiguration configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var orleans = configuration.GetSection("Orleans");
            var section = orleans.GetSection("Providers:Clustering");

            if (section["Provider"] == "AdoNet")
            {
                section = section.GetSection("AdoNet");

                builder
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
    }
}