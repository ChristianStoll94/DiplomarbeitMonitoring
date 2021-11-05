using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.HealthChecks
{
    public class SimpleHealthCheck : IHealthCheck
    {
        private readonly ILogger _logger;

        public SimpleHealthCheck(ILogger<SimpleHealthCheck> logger)
        {
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"{nameof(CheckHealthAsync)}");

            if (true)
            {
                return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
            }

            //return unhealthy as default or overwritten state on startup registration
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result."));
        }
    }
}
