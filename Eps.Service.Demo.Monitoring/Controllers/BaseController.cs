using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _logger.LogInformation("{MethodName} ; Command; {Command} ; UniqueId; {UniqueId} ; Identification; {Identification}", methodName, command.GetType().Name, command.UniqueId, command.Identification);
        }

        protected void LogResponse(string methodName, Response response, DateTime beginExecutionTime, DateTime endExecutionTime)
        {
            _logger.LogInformation("{MethodName} ; Response; {Response} ; ExecutionTime {ExecutionTime}", methodName, ((response == null) ? "NULL" : response.ToString()), (endExecutionTime - beginExecutionTime).ToString("c"));
        }

        protected void ValidateLog(string methodName, Command command, Response response)
        {
            if (command != null && response != null)
            {
                if (command.UniqueId != response.UniqueId)
                {
                    string errorText = "Changed uniqueId detected; " +
                                       "CommandId; " + command.UniqueId.ToString() + "; " +
                                       "ResponseId; " + response.UniqueId.ToString();
                    _logger.LogWarning(nameof(methodName) + "; " + errorText);
                }
            }
        }

    }
}
