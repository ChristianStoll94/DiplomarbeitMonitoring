using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Elastic.Apm;

namespace Eps.Service.Demo.Monitoring.MicroService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        static readonly ActivitySource ActivitySource = new ActivitySource(Assembly.GetExecutingAssembly().GetName().Name);

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet] 
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();

            //var number = GetRandomNumberElasticAPM();
            var number = GetRandomNumberOpenTelemetry();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private int GetRandomNumberElasticAPM()
        {
            var random = new Random();
            return Agent.Tracer.CurrentTransaction.CaptureSpan("GetRandomNumberElasticAPM", "RandomGeneration", (span) =>
            {
                Thread.Sleep(1500);
                span.SetLabel("TestLabel", "I'm a Span");
                return random.Next();
            });
        }

        private int GetRandomNumberOpenTelemetry()
        {
            var random = new Random();

            using (var activity = ActivitySource.StartActivity("GetRandomNumberOpenTelemetry"))
            {
                Thread.Sleep(1500);
                activity.AddTag("TestLabel", "I'm a Span");
                return random.Next();
            }
        }
    }
}
