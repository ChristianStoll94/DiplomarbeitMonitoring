using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Eps.Service.Demo.Monitoring.Workflows;
using Microsoft.Extensions.Hosting;

namespace Eps.Service.Demo.Monitoring.Services
{
    public class HostedServiceOpenTelemetry : BackgroundService
    {
        static ActivitySource s_source = new ActivitySource(typeof(HostedServiceOpenTelemetry).FullName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var activity = s_source.StartActivity("Hosted"))
            {
                await Task.Delay(new TimeSpan(0, 0, 3), stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var activity1 = s_source.StartActivity("SyncTasks"))
                    {
                        Thread.Sleep(3000);

                        SyncWorkflowOpenTelemetry.Task1();
                    }

                    //info: at this version *.-rc8 this activity will not be displayed
                    using (var activity2 = s_source.StartActivity("AsyncTasks"))
                    {
                        await WorkflowOpenTelemetry.DoSomeWork();
                    }

                    await Task.Delay(new TimeSpan(1000), stoppingToken);
                }
            }
        }
    }
}