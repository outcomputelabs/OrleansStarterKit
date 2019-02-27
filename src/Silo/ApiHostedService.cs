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
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading;
using System.Threading.Tasks;

namespace Silo
{
    public class ApiHostedService : IHostedService
    {
        private readonly IWebHost _host;
        private readonly ILogger<ApiHostedService> _logger;

        public ApiHostedService(ILogger<ApiHostedService> logger, IConfiguration configuration, ILoggerProvider loggerProvider, ISiloHostedService silo)
        {
            _logger = logger;

            _host = new WebHostBuilder()

                .UseKestrel(options =>
                {
                    options.ListenAnyIP(configuration.GetValue<int>("Api:Port", 6000));
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

                    configure.AddApiVersioning(options =>
                    {
                        options.ReportApiVersions = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                    });

                    configure.AddVersionedApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                    });

                    configure.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
                    configure.AddSwaggerGen();
                    configure.AddSingleton(silo.ClusterClient);
                })

                .Configure(app =>
                {
                    var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                    app.UseMvc();
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToLowerInvariant());
                        }
                    });
                })

                .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {nameof(ApiHostedService)}...");
            await _host.StartAsync(cancellationToken);
            _logger.LogInformation($"Started {nameof(ApiHostedService)}.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => _host.StopAsync(cancellationToken);
    }
}
