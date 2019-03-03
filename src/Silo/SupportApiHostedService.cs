using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Silo.Api.V1.Controllers;
using Silo.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Silo
{
    /// <summary>
    /// Allows the support API to be hosted as a service in a generic host.
    /// </summary>
    public class SupportApiHostedService : IHostedService
    {
        private readonly IWebHost _host;

        public SupportApiHostedService(IConfiguration configuration, ILoggerProvider loggerProvider, IClusterClient client, INetworkPortFinder portFinder)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (portFinder == null) throw new ArgumentNullException(nameof(portFinder));

            Port = portFinder.GetAvailablePortFrom(
                configuration.GetValue<int>("Api:Port:Start"),
                configuration.GetValue<int>("Api:Port:End"));

            _host = new WebHostBuilder()
                .UseKestrel(op =>
                {
                    op.ListenAnyIP(Port);
                })
                .ConfigureLogging(configure =>
                {
                    configure.AddProvider(loggerProvider);
                })
                .ConfigureServices(configure =>
                {
                    configure.AddMvc()
                        .SetCompatibilityVersion(CompatibilityVersion.Latest)
                        .AddApplicationPart(typeof(DummyController).Assembly)
                        .AddControllersAsServices();

                    configure.AddApiVersioning(op =>
                    {
                        op.ReportApiVersions = true;
                        op.DefaultApiVersion = new ApiVersion(1, 0);
                    });

                    configure.AddVersionedApiExplorer(op =>
                    {
                        op.GroupNameFormat = "'v'VVV";
                    });

                    configure.AddSupportApiInfo(configuration["Api:Title"]);
                    configure.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
                    configure.AddSwaggerGen();
                    configure.AddSingleton(client);
                })
                .Configure(app =>
                {
                    var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                    app.UseMvc();
                    app.UseSwagger();
                    app.UseSwaggerUI(op =>
                    {
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            op.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToLowerInvariant());
                        }
                    });
                })
                .Build();
        }

        public int Port { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}
