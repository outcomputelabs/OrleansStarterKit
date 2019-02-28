using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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

namespace Silo.Services
{
    /// <summary>
    /// Allows the support API to be hosted as a service in a generic host.
    /// </summary>
    public class SupportApiHostedService : IHostedService
    {
        /// <summary>
        /// The web host for the back-end API.
        /// </summary>
        private readonly IWebHost _host;

        /// <summary>
        /// Creates and builds the <see cref="IWebHost"/> for the back-end API as a <see cref="IHostedService"/>.
        /// </summary>
        /// <param name="options">Options to use for building the back-end api.</param>
        /// <param name="loggerProvider">Logger provider to pass to the web host.</param>
        /// <param name="client">Orleans cluster client for the back-end api.</param>
        public SupportApiHostedService(IOptions<ApiOptions> options, ILoggerProvider loggerProvider, IClusterClient client)
        {
            if (options?.Value == null) throw new ArgumentNullException(nameof(options));
            if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));
            if (client == null) throw new ArgumentNullException(nameof(client));

            _host = new WebHostBuilder()

                .UseKestrel(op =>
                {
                    op.ListenAnyIP(options.Value.Port);
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

        public Task StartAsync(CancellationToken cancellationToken) => _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}
