using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;

namespace Eps.Service.Demo.Monitoring
{
    public class MetricsRegistry
    {
        public static CounterOptions SampleCounter => new CounterOptions
        {
            Name = "Sample Counter",
            MeasurementUnit = Unit.Calls,
        };
    }
}
