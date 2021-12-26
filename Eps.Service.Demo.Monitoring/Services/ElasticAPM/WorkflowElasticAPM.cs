using System.Threading.Tasks;
using Elastic.Apm;

namespace Eps.Service.Demo.Monitoring.Services.ElasticAPM
{
    public class WorkflowElasticAPM
    {
        public static async Task RunAsyncTasks()
        {
            await Agent.Tracer.CurrentTransaction.CaptureSpan("AsyncTasks", "Task", async (span) =>
            {
                span.SetLabel("MyLabel", "SomeLabel");
                
                await StepOne();
                await StepTwo();
            });
        }

        public static async Task StepOne()
        {
            await Agent.Tracer.CurrentTransaction.CaptureSpan("StepOne", "Task", async (span) =>
            {
                await Task.Delay(5000);
            });
        }

        public static async Task StepTwo()
        {
            await Agent.Tracer.CurrentTransaction.CaptureSpan("StepTwo", "Task", async (span) =>
            {
                await Task.Delay(3000);
            });
        }
    }
}
