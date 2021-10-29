using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Elastic.Apm;
using Eps.Framework.Exceptions;
using Eps.Service.Demo.Monitoring.API;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eps.Service.Demo.Monitoring.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : BaseController
    {
        private readonly IMetrics _metrics;
        
        public TestController(IMetrics metrics, ILogger<WelcomeController> logger)
            : base(logger)
        {
            _metrics = metrics;
        }

        [HttpPut]
        public TestResponse Create(bool valid, bool throwHandledException, bool throwUnhandledException, bool incrementCounter, string testString)
        {
            return Execute(new TestCommand()
            {
                Valid = valid,
                ThrowHandledException = throwHandledException,
                ThrowUnhandledException = throwUnhandledException,
                IncrementCounter = incrementCounter,
                Teststring = testString
            });
        }

        [HttpPost]
        public TestResponse Execute([FromBody] TestCommand command)
        {
            TestResponse response = null;
            DateTime beginExecutionTime = DateTime.UtcNow;

            int uniqueId = ((command == null) ? -1 : command.UniqueId);

            try
            {
                LogCommand(nameof(Execute), command);
                if (command == null)
                {
                    string errorText = "Command is not supported";
                    _logger.LogError(nameof(Execute) + "; " + errorText);
                    response = new TestResponse(uniqueId, TestResponse.TestErrorCodes.UnknownCommand, errorText);
                }
                else
                {
                    if (command.ThrowUnhandledException)
                    {
                        throw new NullReferenceException("UnhandledTestException");
                    }

                    response = ExecuteCommand(command);
                }
            }
            finally
            {
                DateTime endExecutionTime = DateTime.UtcNow;
                LogResponse(nameof(Execute), response, beginExecutionTime, endExecutionTime);
            }


            ValidateLog(nameof(Execute), command, response);
            return response;
        }

        private TestResponse ExecuteCommand(TestCommand command)
        {
            TestResponse response = ValidateParameters(command);
            if (response.ErrorCode != TestResponse.TestErrorCodes.NoError)
                return response;

            try
            {
                if (command.IncrementCounter)
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.SampleCounter);
                }

                if (command.ThrowHandledException)
                {
                    throw new ExecutionException("Some Exception message");
                }

                
                var randomNumber = GetRandomNumber();
                Agent.Tracer.CurrentTransaction.SetLabel("randomNumber", randomNumber);

                return response;
            }
            catch (ExecutionException ex)
            {
                string errorText = "Unexpected Exception; Message; " + ex.Message;
                _logger.LogError(ex, "{methodName} ; {errorText}", nameof(Execute), errorText);
                response = new TestResponse(command.UniqueId, TestResponse.TestErrorCodes.ExecutionError, ex.Content.ToMessage());
            }

            return response;
        }

        private int GetRandomNumber()
        {
            var random = new Random();
            return Agent.Tracer.CurrentTransaction.CaptureSpan("TestSpan", "RandomGeneration", (span) =>
            {
                Thread.Sleep(1500);
                span.SetLabel("TestLabel", "I'm a Span");
                span.SetLabel("TestLabel2", "I'm a Span");
                return random.Next();
            });
        }

        private TestResponse ValidateParameters(TestCommand command)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("{ValidateParameters}; Command; {command}", nameof(ValidateParameters), command);

            if (!command.Valid)
            {
                string errorText = "Invalid Teststring";
                string data = (command.Teststring ?? "NULL");
                _logger.LogWarning("{ValidateParameters} ; {errorText} ; Data; {data}", nameof(ValidateParameters) , errorText, data);
                return new TestResponse(command.UniqueId, TestResponse.TestErrorCodes.InvalidParameter, errorText + " Data; " + data);
            }

            return new TestResponse(command.UniqueId, TestResponse.TestErrorCodes.NoError, string.Empty);
        }
    }
}
