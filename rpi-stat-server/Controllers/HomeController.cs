using System;
using System.Data.SqlClient;
using core;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace rpi_stat_server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) =>
            _logger = logger;

        public IActionResult Test() =>
            Content("OK");

        [HttpPost]
        public IActionResult AddReading([FromBody] Reading reading)
        {
            using (var conn = new SqlConnection("Server=localhost;Database=rpi_stat;Trusted_Connection=yes;"))
            {
                var data = new TemperatureReading {
                    Timestamp = DateTime.Now,
                    LocationID = reading.LocationID,
                    Temperature = reading.Temperature
                };

                conn.Insert(data);
            }

            return Content(JsonConvert.SerializeObject(reading, Formatting.Indented));
        }
    }
}
