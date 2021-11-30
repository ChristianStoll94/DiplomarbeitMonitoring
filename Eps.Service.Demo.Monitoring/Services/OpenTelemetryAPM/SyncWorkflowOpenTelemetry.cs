using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Eps.Service.Demo.Monitoring.Services.OpenTelemetryAPM
{
    public class SyncWorkflowOpenTelemetry
    {
        static readonly ActivitySource ActivitySource = new ActivitySource(Assembly.GetExecutingAssembly().GetName().Name);

        public static void RunSyncTasks()
        {
            using (var activity = ActivitySource.StartActivity("RunSyncTasks"))
            {
                Task1();
                Task2();

                Thread.Sleep(1000);
            }
        }

        public static void Task1()
        {
            using (var activity = ActivitySource.StartActivity("Task1"))
            {
                Thread.Sleep(3000);
            }
        }

        public static void Task2()
        {
            using (var activity = ActivitySource.StartActivity("Task2"))
            {
                Thread.Sleep(2000);

                if (StaticTestProperties.ThrowException)
                    throw new Exception("TestException");
            }
        }
    }
}