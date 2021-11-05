using System.Threading;
using Elastic.Apm;

namespace Eps.Service.Demo.Monitoring.Workflows
{
    public class SyncWorkflow
    {
       
        public static void Task1()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("Task1", "Task", (span) =>
            {
                Task2();
                Task3();
            });
        }

        public static void Task2()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("Task2", "Task", (span) =>
            {
                Thread.Sleep(3000);
            });
        }

        public static void Task3()
        {
            Agent.Tracer.CurrentTransaction.CaptureSpan("Task3", "Task", (span) =>
            {
                Thread.Sleep(3000);
            });
        }
    }
}
