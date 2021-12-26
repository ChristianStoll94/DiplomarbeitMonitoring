using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using Eps.Service.Extensions.Monitoring;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Services.ElasticAPM
{
    public class HostedServiceElasticAPM : BackgroundService
    {
        readonly ILogger _logger;

        public HostedServiceElasticAPM(ILogger<HostedServiceElasticAPM> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Agent.Tracer.CaptureTransaction(this.GetType().Name, "HostedService", async (transaction) =>
                {
                    try
                    {
                        transaction.SetLabel("MyLabel", "SomeLabel");

                        SyncWorkflowElasticAPM.RunSyncTasks();

                        await WorkflowElasticAPM.RunAsyncTasks();
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
