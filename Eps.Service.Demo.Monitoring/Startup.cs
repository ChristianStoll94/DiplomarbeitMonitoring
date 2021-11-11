using System;
using System.Linq;
using System.Reflection;
using Eps.Framework.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Eps.Service.Extensions.Logging;
using Eps.Service.Extensions.Monitoring;
using Eps.Service.Extensions.Swagger;
using Microsoft.Extensions.Logging;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Eps.Service.Demo.Monitoring.HealthChecks;
using Eps.Service.Demo.Monitoring.Services;
using Eps.Service.Demo.Monitoring.Workflows;
using Eps.Service.Extensions.Health;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
            services.AddHostedService<HostedService>();

            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddSingleton(new AssemblyReader(Assembly.GetExecutingAssembly()));

            services.AddLogging(Configuration);

            services.AddControllers();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true);
            
            services.AddOpenTelemetryTracing(builder => builder
                .AddSource(typeof(WorkflowOpenTelemetry).FullName, typeof(SyncWorkflowOpenTelemetry).FullName, typeof(HostedServiceOpenTelemetry).FullName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(Assembly.GetExecutingAssembly().GetName().Name))
                .AddAspNetCoreInstrumentation()
                .AddElasticsearchClientInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:8200");
                    o.Headers = "Authorization=ApiKey LU9MejZYd0J6V3JpeTdyWnFjQXg6RVVyWm8weE9UVmlNR0dkaUEzeDBndw==";
                }));

            //Metrics config
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(options =>
                {
                    options.Enabled = true;
                })
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();
            
            //Metrics registration
            services.AddMetrics(metrics);
            services.AddAppMetricsHealthPublishing();
            services.AddMetricsEndpoints(options =>
                options.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters
                    .OfType<MetricsPrometheusTextOutputFormatter>().First());
            services.AddMetricsEndpoints(options =>
                options.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters
                    .OfType<MetricsPrometheusProtobufOutputFormatter>().First());

            //Healthy registration
            services.AddHealth(Configuration)
                .AddCheck<SimpleHealthCheck>($"{nameof(SimpleHealthCheck)}");

            services.AddSwagger(new AssemblyReader(Assembly.GetExecutingAssembly()), Configuration);

            services.AddHostedService<HostedServiceOpenTelemetry>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(ILogger<Startup> logger, IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, AssemblyReader assemblyReader)
        {
            try
            {
                app.LogStartup(logger, assemblyReader);
                app.UseLogging(loggerFactory);

                app.UseMonitoring(Configuration);

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
