using System;
using System.Reflection;
using Eps.Framework.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Eps.Service.Extensions.Logging;
using Eps.Service.Extensions.Monitoring.OpenTelemetry;
using Eps.Service.Extensions.Swagger;
using Microsoft.Extensions.Logging;
using Eps.Service.Demo.Monitoring.HealthChecks;
using Eps.Service.Demo.Monitoring.Services.OpenTelemetryAPM;
using Eps.Service.Extensions.Health;

namespace Eps.Service.Demo.Monitoring
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
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddSingleton(new AssemblyReader(Assembly.GetExecutingAssembly()));

            services.AddLogging(Configuration);

            services.AddControllers();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true);

            services.AddOpenTelemetryMonitoring(Configuration);

            services.AddMetrics(Configuration);

            //Healthy registration
            services.AddHealth(Configuration)
                .AddCheck<SimpleHealthCheck>($"{nameof(SimpleHealthCheck)}");

            //ElasticAPM Background Service
            //services.AddHostedService<HostedServiceElasticAPM>();

            services.AddHostedService<HostedServiceOpenTelemetry>();

            services.AddSwagger(new AssemblyReader(Assembly.GetExecutingAssembly()), Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(ILogger<Startup> logger, IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, AssemblyReader assemblyReader)
        {
            try
            {
                app.LogStartup(logger, assemblyReader);

                app.UseLogging(loggerFactory);
                app.UseOpenTelemetryLoggingMiddleware();

                //Elastic APM Monitoring
                //app.UseMonitoring(Configuration);

                app.UseMetricsAllEndpoints();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealth();
                });

                app.UseSwagger(assemblyReader, Configuration);
            }
            catch (Exception ex)
            {
                string errorText = "Unexpected Exception; Message; " + ex.Message;
                logger.LogCritical(ex, errorText);
                throw;
            }
        }
    }
}
