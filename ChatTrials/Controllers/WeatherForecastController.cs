using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatTrials.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IHubContext<ChartHub> _hubContext;
        private readonly ChartHub _chartHub;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                        ChartHub chartHub, IHubContext<ChartHub> hubContext)
        {
            _logger = logger;
            _chartHub = chartHub;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("send")]
        public async Task<IActionResult> Send()
        {
            //var loggedInUserName = User.FindFirst(ClaimTypes.Name).Value;
            //var loggedInUserName3 = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ChartHub.data.TryGetValue("3alaAllah", out string connId);
            await _hubContext.Clients.Client(connId).SendAsync("newData", 4);
            return Ok();
        }
    }
}