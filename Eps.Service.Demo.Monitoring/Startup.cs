using System;
using System.Linq;
using System.Reflection;
using Eps.Framework.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Eps.Service.Extensions.Logging;
using Eps.Service.Extensions.Monitoring;
using Eps.Service.Extensions.Swagger;
using Eps.Service.Extensions.Validation;
using Microsoft.Extensions.Logging;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Elastic.Apm.NetCoreAll;
using Elastic.CommonSchema.Serilog;
using Eps.Service.Demo.Monitoring.Services;
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
            services.AddHostedService<HostedService>();

            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddSingleton(new AssemblyReader(Assembly.GetExecutingAssembly()));

            services.AddLogging(Configuration);

            services.AddControllers();

            //fluent validation
            services.AddValidation();
            //services.AddTransient<IValidator<TestCommand>, TestCommandValidator>();

            //Metrics registration
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(options =>
                {
                    options.Enabled = true;
                    options.ReportingEnabled = true;
                })
                //.Report.ToTextFile(@"C:\Projects\metrics.txt", TimeSpan.FromSeconds(4))
                //.OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();
            services.AddMetrics(metrics);
            services.AddMetricsReportingHostedService();
            services.AddAppMetricsHealthPublishing();
            //services.AddMetricsEndpoints(options =>
            //    options.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters
            //        .OfType<MetricsPrometheusTextOutputFormatter>().First());
            services.AddMetricsEndpoints(options =>
                options.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters
                    .OfType<MetricsPrometheusProtobufOutputFormatter>().First());

            services.AddHealth(Configuration);

            services.AddSwagger(new AssemblyReader(Assembly.GetExecutingAssembly()), Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(ILogger<Startup> logger, IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, AssemblyReader assemblyReader)
        {
            try
            {
                app.LogStartup(logger, assemblyReader);
                app.UseLogging(loggerFactory);

                app.UseMonitoring(Configuration);

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                
                app.UseRouting();
                app.UseAuthorization();

                app.UseMetricsAllEndpoints();

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
