using App.Metrics;
using App.Metrics.Counter;


namespace Eps.Service.Demo.Monitoring.Metrics
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
