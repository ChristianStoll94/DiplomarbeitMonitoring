using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Eps.Service.Demo.Monitoring.Services.OpenTelemetryAPM
{

    public static class WorkflowOpenTelemetry
    {
        static readonly ActivitySource ActivitySource = new ActivitySource(Assembly.GetExecutingAssembly().GetName().Name);

        public static async Task RunAsyncTasks()
        {
            using (var activity = ActivitySource.StartActivity("RunAsyncTasks"))
            {
                activity.AddTag("MyTag", "SomeTag");

                await Task.Delay(5000);
            }
        }

        public static async Task StepOne()
        {
            using (var activity = ActivitySource.StartActivity())
            {
                await Task.Delay(5000);
            }
        }

        public static async Task StepTwo()
        {
            using (var activity = ActivitySource.StartActivity())
            {
                await Task.Delay(3000);
            }
        }
    }
}