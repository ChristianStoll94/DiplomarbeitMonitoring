using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading;
using App.Metrics;
using Elastic.Apm;
using Eps.Framework.Exceptions;
using Eps.Service.Demo.Monitoring.API;
using Eps.Service.Demo.Monitoring.Metrics;
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
                ExampleParameter = testString
            });
        }
        
        
        private TestResponse Execute(TestCommand command)
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
                    _logger.LogError("{MethodName}; {@Data}", nameof(Execute), new {ErrorText = errorText});
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
            
            return response;
        }

        private TestResponse ExecuteCommand(TestCommand command)
        {
            TestResponse response = ValidateParameters(command);
            if (response.ErrorCode != TestResponse.TestErrorCodes.NoError)
                return response;

            var a = Activity.Current?.TraceId;
            var b = Activity.Current?.SpanId;

            try
            {
                if (command.IncrementCounter)
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.SampleCounter);
                }

                if (command.ThrowHandledException)
                {
                    throw new ExecutionException("ExecutionException");
                }

                //var randomNumber = GetRandomNumber();
                //Agent.Tracer.CurrentTransaction.SetLabel("RandomNumber", randomNumber);

                return response;
            }
            catch (ExecutionException ex)
            {
                _logger.LogError(ex, "{MethodName}; Data; {@Data}", nameof(Execute), new {Command = command, ErrorText = ex.Message});
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
                return random.Next();
            });
        }

        private TestResponse ValidateParameters(TestCommand command)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("{MethodName}; Command; {@Data}", nameof(ValidateParameters), new {Command = command});

            if (!command.Valid)
            {
                string errorText = "Invalid Parameter";
                string invalidParameter = (command.ExampleParameter ?? "NULL");
                _logger.LogWarning("{MethodName}; Data; {@Data}", nameof(ValidateParameters) , new {Message = errorText, Command = command, InvalidParameter = invalidParameter });
                return new TestResponse(command.UniqueId, TestResponse.TestErrorCodes.InvalidParameter, errorText + " Data; " + invalidParameter);
            }

            return new TestResponse(command.UniqueId, TestResponse.TestErrorCodes.NoError, string.Empty);
        }
    }
}
