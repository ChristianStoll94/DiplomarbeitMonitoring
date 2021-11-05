using System;
using Eps.Service.Demo.Monitoring.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected void LogCommand(string methodName, Command command)
        {
            _logger.LogInformation("{MethodName}; Data; {@Data}", methodName, new { Command = command});
        }

        protected void LogResponse(string methodName, Response response, DateTime beginExecutionTime, DateTime endExecutionTime)
        {
            _logger.LogInformation("{MethodName}; Data; {@Data}; Diagnostics {@Diagnostics}", methodName, new { Response = response}, new {ExecutionTime = (endExecutionTime - beginExecutionTime).ToString("c")});
        }

        protected void ValidateLog(string methodName, Command command, Response response)
        {
            _logger.LogInformation("{MethodName}; Data; {@Data};", nameof(ValidateLog), new { Command = command, Response = response });

            if (command != null && response != null)
            {
                if (command.UniqueId != response.UniqueId)
                {
                    _logger.LogWarning("{MethodName}; Data; {@Data};", methodName, new { Message = "Changed uniqueId detected", CommandUniqueId = command.UniqueId, ResponseUniqueId = command.UniqueId });
                }
            }
        }

    }

}
