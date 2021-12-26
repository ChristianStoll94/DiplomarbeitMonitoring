using System;
using System.Net.Http;
using System.Threading.Tasks;
using Eps.Framework.Reflection;
using Eps.Service.Demo.Monitoring.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestAsyncController : BaseController
    {
        private readonly AssemblyReader _assemblyHelper;
        static readonly HttpClient Client = new HttpClient();

        public TestAsyncController(ILogger<WelcomeController> logger)
            : base(logger)
        {
        }

        [HttpGet]
        public async Task<TestAsyncResponse> Get()
        {
            return await Execute(new TestAsyncCommand());
        }

        [HttpPost]
        public async Task<TestAsyncResponse> Execute([FromBody] TestAsyncCommand command)
        {
            TestAsyncResponse response = null;

            int uniqueId = ((command == null) ? -1 : command.UniqueId);

            try
            {
                LogCommand(nameof(Execute), command);
                if (command == null)
                {
                    string errorText = "Command is not supported";
                    _logger.LogError("{MethodName}; Data; {@Data}", nameof(Execute), new { ErrorText = errorText });
                    response = new TestAsyncResponse(uniqueId, TestAsyncResponse.TestAsyncErrorCodes.UnknownCommand, errorText);
                }
                else
                {
                    response = await ExecuteCommand(command);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{MethodName} ; {@Data}", nameof(Execute), new { ErrorText = "Unexpected Exception" });

                response = new TestAsyncResponse(uniqueId, TestAsyncResponse.TestAsyncErrorCodes.UnexpectedException, ex.Message);
            }
            finally
            {
                LogResponse(nameof(Execute), response);
            }

            return response;
        }

        private async Task<TestAsyncResponse> ExecuteCommand(TestAsyncCommand command)
        {
            TestAsyncResponse response = await ValidateParameters(command);
            if (response.ErrorCode != TestAsyncResponse.TestAsyncErrorCodes.NoError)
                return response;

            HttpResponseMessage httpResponseMessage = await Client.GetAsync("http://localhost:48060/WeatherForecast");
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();

            return new TestAsyncResponse(command.UniqueId, TestAsyncResponse.TestAsyncErrorCodes.NoError, string.Empty)
            {
                ResponseString = responseString
            };
        }

        private async Task<TestAsyncResponse> ValidateParameters(TestAsyncCommand command)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("{MethodName}; Data; {@Data}", nameof(ValidateParameters), new { Command = command });



            return new TestAsyncResponse(command.UniqueId, TestAsyncResponse.TestAsyncErrorCodes.NoError, string.Empty);
        }
    }
}
