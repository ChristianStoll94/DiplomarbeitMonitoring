﻿using System.Diagnostics;
using System.Threading.Tasks;

namespace Eps.Service.Demo.Monitoring.Workflows
{
    public static class WorkflowOpenTelemetry
    {
        static WorkflowOpenTelemetry()
        {
            var a = 0;
        }

        static ActivitySource s_source = new ActivitySource(typeof(WorkflowOpenTelemetry).FullName);

        public static async Task DoSomeWork()
        {
            using (var activity = s_source.StartActivity("DoSomeWork"))
            {
                //set additional tags
                activity.AddTag("MyTagId", "ValueInTags");

                await StepOne();
                await StepTwo();
            }
        }

        public static async Task StepOne()
        {
            using (var activity = s_source.StartActivity("StepOne"))
            {
                await Task.Delay(5000);
            }
        }

        public static async Task StepTwo()
        {
            using (var activity = s_source.StartActivity("StepTwo"))
            {
                await Task.Delay(3000);
            }
        }
    }
}