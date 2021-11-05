using System.Threading.Tasks;
using Elastic.Apm;

namespace Eps.Service.Demo.Monitoring.Workflows
{
    public class Workflow
    {
        public static async Task DoSomeWork()
        {
            await Agent.Tracer.CurrentTransaction.CaptureSpan("SomeWork", "Task", async (span) =>
            {
                span.SetLabel("TestLabel", "Label1");
                
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
