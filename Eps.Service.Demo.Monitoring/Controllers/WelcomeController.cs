using System;
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
        private readonly AssemblyReader _assemblyHelper;

        public WelcomeController(ILogger<WelcomeController> logger, AssemblyReader assemblyHelper)
            : base(logger)
        {
            _assemblyHelper = assemblyHelper;
        }

        [HttpGet]
        public WelcomeResponse Get()
        {
            return Execute(new WelcomeCommand());
        }

        [HttpPost]
        public WelcomeResponse Execute([FromBody] WelcomeCommand command)
        {
            WelcomeResponse response = null;
            DateTime beginExecutionTime = DateTime.UtcNow;

            int uniqueId = ((command == null) ? -1 : command.UniqueId);
            
                try
                {
                    LogCommand(nameof(Execute), command);
                    if (command == null)
                    {
                        string errorText = "Command is not supported";
                        _logger.LogError("{MethodName} ; {@Data}", nameof(Execute), new { ErrorText = errorText });
                        response = new WelcomeResponse(uniqueId, WelcomeResponse.WelcomeErrorCodes.UnknownCommand, errorText);
                    }
                    else
                    {
                        response = ExecuteCommand(command);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{methodName} ; {ErrorType} ; {ErrorText}", nameof(Execute), "Unexpected Exception", ex.Message);
                    response = new WelcomeResponse(uniqueId, WelcomeResponse.WelcomeErrorCodes.UnexpectedException, ex.Message);
                }
                finally
                {
                    DateTime endExecutionTime = DateTime.UtcNow;
                    LogResponse(nameof(Execute), response, beginExecutionTime, endExecutionTime);
                }

                return response;
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
                _logger.LogDebug("{MethodName}; Command; {@Data}", nameof(ValidateParameters), new { Command = command });

            return new WelcomeResponse(command.UniqueId, WelcomeResponse.WelcomeErrorCodes.NoError, string.Empty);
        }
    }
}
