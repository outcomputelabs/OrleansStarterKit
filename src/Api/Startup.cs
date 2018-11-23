using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;
using IAspNetHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // configure serilog
            var logger = new LoggerConfiguration()
                .WriteTo.Console(
                    restrictedToMinimumLevel: Configuration.GetValue<LogEventLevel>("Serilog:Console:RestrictedToMinimumLevel"))
                .WriteTo.MSSqlServer(
                    connectionString: Configuration.GetConnectionString("Logging"),
                    schemaName: Configuration.GetValue<string>("Serilog:MSSqlServer:SchemaName"),
                    tableName: Configuration.GetValue<string>("Serilog:MSSqlServer:TableName"),
                    restrictedToMinimumLevel: Configuration.GetValue<LogEventLevel>("Serilog:MSSqlServer:RestrictedToMinimumLevel"))
                .CreateLogger();

            // add serilog to services
            services.AddLogging(config => config.AddSerilog(logger));

            // configure and add the orleans client
            services.AddSingleton(new ClientBuilder()
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString(Configuration["Orleans:Clustering:AdoNet:ConnectionStringName"]);
                    options.Invariant = Configuration["Orleans:Clustering:AdoNet:Invariant"];
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = Configuration["Orleans:ClusterId"];
                    options.ServiceId = Configuration["Orleans:ServiceId"];
                })
                .ConfigureLogging(config =>
                {
                    config.AddSerilog(logger);
                })
                .Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IAspNetHostingEnvironment env, ILogger<Startup> logger, IClusterClient client)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            // connect to the orleans cluster
            logger.LogInformation("Connecting to the Orleans cluster...");
            var attempt = 1;
            client.Connect(async error =>
            {
                logger.LogWarning("Failed to connect to the Orleans cluster on attempt {@Attempt}.", attempt);
                await Task.Delay(TimeSpan.FromSeconds(1));
                ++attempt;
                return true;
            }).Wait();
            logger.LogInformation("Connected to the Orleans cluster after {@Attempts} attempt(s).", attempt);
        }
    }
}
