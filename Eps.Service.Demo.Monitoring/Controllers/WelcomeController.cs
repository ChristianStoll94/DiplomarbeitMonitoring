using System;
using App.Metrics;
using Eps.Framework.Reflection;
using Eps.Service.Demo.Monitoring.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WelcomeController : BaseController
    {
        private AssemblyReader _assemblyHelper;
        private readonly IMetrics _metrics;

        public WelcomeController(IMetrics metrics, ILogger<WelcomeController> logger, AssemblyReader assemblyHelper)
            : base(logger)
        {
            _assemblyHelper = assemblyHelper;
            _metrics = metrics;
        }

        [HttpGet]
        public WelcomeResponse Get()
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.SampleCounter);
            return Execute(new WelcomeCommand());
        }

        [HttpPost]
        public WelcomeResponse Execute([FromBody] WelcomeCommand command)
        {
            WelcomeResponse response = null;
            DateTime beginExecutionTime = DateTime.UtcNow;

            int uniqueId = ((command == null) ? -1 : command.UniqueId);
            using (_logger.BeginScope("UniqueId", uniqueId))
            {
                try
                {
                    LogCommand(nameof(Execute), command);
                    if (command == null)
                    {
                        string errorText = "Command is not supported";
                        _logger.LogError(nameof(Execute) + "; " + errorText);
                        response = new WelcomeResponse(uniqueId, WelcomeResponse.WelcomeErrorCodes.UnknownCommand, errorText);
                    }
                    else
                    {
                        response = ExecuteCommand(command);
                    }
                }
                catch (Exception ex)
                {
                    string errorText = "Unexpected Exception; Message; " + ex.Message;
                    _logger.LogError(ex, nameof(Execute) + "; " + errorText);
                    response = new WelcomeResponse(uniqueId, WelcomeResponse.WelcomeErrorCodes.UnexpectedException, errorText);
                }
                finally
                {
                    DateTime endExecutionTime = DateTime.UtcNow;
                    LogResponse(nameof(Execute), response, beginExecutionTime, endExecutionTime);
                }

                return response;
            }
        }

        private WelcomeResponse ExecuteCommand(WelcomeCommand command)
        {
            WelcomeResponse response = ValidateParameters(command);
            if (response.ErrorCode != WelcomeResponse.WelcomeErrorCodes.NoError)
                return response;

            return new WelcomeResponse(command.UniqueId, WelcomeResponse.WelcomeErrorCodes.NoError, string.Empty, _assemblyHelper.Version.ToString());
        }

        private WelcomeResponse ValidateParameters(WelcomeCommand command)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug(nameof(ValidateParameters) + "; Command; " + command.ToString());

            return new WelcomeResponse(command.UniqueId, WelcomeResponse.WelcomeErrorCodes.NoError, string.Empty);
        }
    }
}
