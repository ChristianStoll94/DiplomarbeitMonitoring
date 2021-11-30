using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using Eps.Service.Extensions.Monitoring;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Services.ElasticAPM
{
    public class HostedService : BackgroundService
    {
        readonly ILogger _logger;
        private readonly IMonitoringProvider _monitoringProvider;

        public HostedService(ILogger<HostedService> logger, IMonitoringProvider monitoringProvider)
        {
            _logger = logger;
            _monitoringProvider = monitoringProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //ToDo: Do better
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Agent.Tracer.CaptureTransaction(this.GetType().Name, "HostedService", async (transaction) =>
                {
                    try
                    {
                        _logger.LogInformation("{APMSource}", "Elastic");

                        transaction.SetLabel("MyLabel", "SomeLabel");

                        SyncWorkflow.RunSyncTasks();

                        await Workflow.RunAsyncTasks();
                    }
                    catch (Exception ex)
                    {
                        Elastic.Apm.Agent.Tracer.CurrentTransaction?.CaptureException(ex);
                    }
                });
            }
        }
    }
}
