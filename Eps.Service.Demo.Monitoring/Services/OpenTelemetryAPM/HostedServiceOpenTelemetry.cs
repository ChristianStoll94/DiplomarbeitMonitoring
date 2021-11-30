using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eps.Service.Extensions.Monitoring;
using Eps.Service.Extensions.Monitoring.OpenTelemetry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace Eps.Service.Demo.Monitoring.Services.OpenTelemetryAPM
{
    public class HostedServiceOpenTelemetry : BackgroundService
    {
        protected readonly ILogger _logger;
        private readonly IMonitoringProvider _monitoringProvider;

        public HostedServiceOpenTelemetry(ILogger<HostedServiceOpenTelemetry> logger, IMonitoringProvider monitoringProvider)
        {
            _logger = logger;
            _monitoringProvider = monitoringProvider;

        }

        static readonly ActivitySource ActivitySource = new ActivitySource(Assembly.GetExecutingAssembly().GetName().Name);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var activity = ActivitySource.StartActivity(this.GetType().Name))
                {
                    try
                    {
                        using (_logger.BeginElasticLogging())
                        {
                            _logger.LogInformation("{MethodName}; {@Data}; {APMSource}", nameof(ExecuteAsync),
                                new {Test = "test"}, "OpenTelemetry");

                            activity.AddTag("MyTag", "SomeTag");

                            SyncWorkflowOpenTelemetry.RunSyncTasks();

                            await WorkflowOpenTelemetry.RunAsyncTasks();
                        }
                    }
                    catch (Exception ex)
                    {
                        Activity.Current?.RecordException(ex);
                    }
                }
            }
        }
    }
}