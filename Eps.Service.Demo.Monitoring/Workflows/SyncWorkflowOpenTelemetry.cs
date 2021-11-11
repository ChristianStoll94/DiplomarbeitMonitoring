using System.Diagnostics;
using System.Threading;

namespace Eps.Service.Demo.Monitoring.Workflows
{
    public class SyncWorkflowOpenTelemetry
    {
        static ActivitySource s_source = new ActivitySource(typeof(SyncWorkflowOpenTelemetry).FullName);

        public static void Task1()
        {
            using (var activity = s_source.StartActivity("Task1"))
            {
                Task2();
                Task3();

                Thread.Sleep(1000);
            }
        }

        public static void Task2()
        {
            using (var activity = s_source.StartActivity("Task2"))
            {
                Thread.Sleep(3000);
            }
        }

        public static void Task3()
        {
            using (var activity = s_source.StartActivity("Task3"))
            {
                Thread.Sleep(2000);
            }
        }
    }
}