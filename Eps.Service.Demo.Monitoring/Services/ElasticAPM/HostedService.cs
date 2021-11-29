using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Services.ElasticAPM
{
    public class HostedService : BackgroundService
    {
        readonly ILogger _logger;

        public HostedService(ILogger<HostedService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //ToDo: Do better
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Agent.Tracer.CaptureTransaction(this.GetType().Name, "HostedService", async (transaction) =>
                {
                    _logger.LogInformation("{APMSource}", "Elastic");

                    transaction.SetLabel("MyLabel", "SomeLabel");

                    SyncWorkflow.RunSyncTasks();

                    await Workflow.RunAsyncTasks();
                });
            }
        }
    }
}
