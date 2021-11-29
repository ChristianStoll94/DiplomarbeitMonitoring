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
            DateTime beginExecutionTime = DateTime.UtcNow;

            int uniqueId = ((command == null) ? -1 : command.UniqueId);

            try
            {
                LogCommand(nameof(Execute), command);
                if (command == null)
                {
                    string errorText = "Command is not supported";
                    _logger.LogError("{MethodName} ; {@Data}", nameof(Execute), new { ErrorText = errorText });
                    response = new TestAsyncResponse(uniqueId, TestAsyncResponse.TestAsyncErrorCodes.UnknownCommand, errorText);
                }
                else
                {
                    response = await ExecuteCommand(command);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{methodName} ; {ErrorType} ; {ErrorText}", nameof(Execute), "Unexpected Exception", ex.Message);
                response = new TestAsyncResponse(uniqueId, TestAsyncResponse.TestAsyncErrorCodes.UnexpectedException, ex.Message);
            }
            finally
            {
                DateTime endExecutionTime = DateTime.UtcNow;
                LogResponse(nameof(Execute), response, beginExecutionTime, endExecutionTime);
            }

            return response;
        }

        private async Task<TestAsyncResponse> ExecuteCommand(TestAsyncCommand command)
        {
            TestAsyncResponse response = await ValidateParameters(command);
            if (response.ErrorCode != TestAsyncResponse.TestAsyncErrorCodes.NoError)
                return response;

            HttpResponseMessage response2 = await Client.GetAsync("http://localhost:48060/WeatherForecast");
            response2.EnsureSuccessStatusCode();
            string responseBody = await response2.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
            return new TestAsyncResponse(command.UniqueId, TestAsyncResponse.TestAsyncErrorCodes.NoError, string.Empty)
            {
                XXX = responseBody
            };
        }

        private async Task<TestAsyncResponse> ValidateParameters(TestAsyncCommand command)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("{MethodName}; Command; {@Data}", nameof(ValidateParameters), new { Command = command });



            return new TestAsyncResponse(command.UniqueId, TestAsyncResponse.TestAsyncErrorCodes.NoError, string.Empty);
        }
    }
}
