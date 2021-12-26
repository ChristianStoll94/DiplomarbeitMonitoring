using System;
using System.Threading;
using Elastic.Apm;
using Eps.Service.Demo.Monitoring.Services.OpenTelemetryAPM;

namespace Eps.Service.Demo.Monitoring.Services.ElasticAPM
{
    public class SyncWorkflowElasticAPM
    {
        public static void RunSyncTasks()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("RunSyncTasks", "Task", (span) =>
            {
                Task1();
                Task2();

                Thread.Sleep(1000);
            });
        }

        public static void Task1()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("Task1", "Task", (span) =>
            {
                Thread.Sleep(3000);
            });
        }

        public static void Task2()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("Task2", "Task", (span) =>
            {
                Thread.Sleep(3000);
                if (StaticTestProperties.ThrowException)
                    throw new Exception("TestException");

            });
        }
    }
}
