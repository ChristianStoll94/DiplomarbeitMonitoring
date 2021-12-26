using System;
using Eps.Service.Demo.Monitoring.API;
using Eps.Service.Extensions.Monitoring.OpenTelemetry;
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
            _logger.LogInformation("{MethodName}; Data; {@Data}", methodName, new {Command = command});
        }

        protected void LogResponse(string methodName, Response response)
        {
            _logger.LogInformation("{MethodName}; Data; {@Data}", methodName, new {Response = response});
        }

        protected void ValidateLog(string methodName, Command command, Response response)
        {
            _logger.LogInformation("{MethodName}; Data; {@Data}", methodName, new {Command = command});
            _logger.LogInformation("{MethodName}; Data; {@Data}", methodName, response);


            _logger.LogInformation("{MethodName}; Data; {@Data};", nameof(ValidateLog),
                    new {Command = command, Response = response});



                if (command != null && response != null)
                {
                    if (command.UniqueId != response.UniqueId)
                    {
                        _logger.LogWarning("{MethodName}; Data; {@Data};", methodName,
                            new
                            {
                                Message = "Changed uniqueId detected", 
                                CommandUniqueId = command.UniqueId,
                                ResponseUniqueId = response.UniqueId
                            });
                    }
                }
        }

    }

}
