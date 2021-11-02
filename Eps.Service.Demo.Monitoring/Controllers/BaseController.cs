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
            _logger.LogInformation("{MethodName} ; Command; {@LoggingObject}", methodName, command);
        }

        protected void LogResponse(string methodName, Response response, DateTime beginExecutionTime, DateTime endExecutionTime)
        {
            _logger.LogInformation("{MethodName} ; Response; {@LoggingObject} ; ExecutionTime {ExecutionTime}", methodName, response, (endExecutionTime - beginExecutionTime).ToString("c"));
        }

        protected void ValidateLog(string methodName, Command command, Response response)
        {
            if (command != null && response != null)
            {
                if (command.UniqueId != response.UniqueId)
                {
                    _logger.LogWarning("{MethodName} ; Message; {Message}; CommandId; {CommandId}; ResponseId {ResponseId}", nameof(methodName), "Changed uniqueId detected", command.UniqueId, response.UniqueId);
                }
            }
        }

    }
}
