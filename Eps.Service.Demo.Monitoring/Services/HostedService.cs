using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using Eps.Service.Demo.Monitoring.Workflows;
using Microsoft.Extensions.Hosting;

namespace Eps.Service.Demo.Monitoring.Services
{
    public class HostedService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //ToDo: Do better
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Agent.Tracer.CaptureTransaction("Execute DoSomeWork", "HostedService", async (transaction) =>
                {
                    transaction.SetLabel("Testlabel", "Label1");

                    await Task.Delay(new TimeSpan(0, 0, 3), stoppingToken);

                    Thread.Sleep(3000);
                    SyncWorkflow.Task1();
                    
                    
                    await Workflow.DoSomeWork();


                    await Task.Delay(new TimeSpan(1000), stoppingToken);

                });
            }
        }
    }
}
