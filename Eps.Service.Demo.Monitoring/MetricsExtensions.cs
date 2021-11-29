using System.Linq;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eps.Service.Demo.Monitoring
{
    public static class MetricsExtensions
    {
        private const string MetricsSettings = "Metrics";

        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            MetricsExtensionOptions options = configuration.GetSection(MetricsSettings).Get<MetricsExtensionOptions>();
            return services.AddMetrics(options);
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services, MetricsExtensionOptions options)
        {
            AddMetrics2(services, options);
            return services;
        }

        public static void AddMetrics2(IServiceCollection services, MetricsExtensionOptions options)
        {
            if (options.IsEnabled)
            {
                //Metrics config
                var metrics = AppMetrics.CreateDefaultBuilder()
                    .Configuration.Configure(options =>
                    {
                        options.Enabled = true;
                    })
                    .OutputMetrics.AsPrometheusPlainText()
                    .OutputMetrics.AsPrometheusProtobuf()
                    .Build();

                //Metrics registration
                services.AddMetrics(metrics);
                services.AddAppMetricsHealthPublishing();
                services.AddMetricsEndpoints(options =>
                    options.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters
                        .OfType<MetricsPrometheusTextOutputFormatter>().First());
                services.AddMetricsEndpoints(options =>
                    options.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters
                        .OfType<MetricsPrometheusProtobufOutputFormatter>().First());
            }
        }
    }

    public class MetricsExtensionOptions
    {
        public bool IsEnabled { get; set; }
    }
}
