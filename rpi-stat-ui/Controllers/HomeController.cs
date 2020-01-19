using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rpi_stat_ui.Models;

namespace rpi_stat_ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) =>
            _logger = logger;

        public IActionResult Index() =>
            View(new IndexViewModel());

        public IActionResult Test() =>
            Content("OK");

        [HttpPost]
        public IActionResult AddReading([FromBody] Reading reading)
        {
            _logger.LogInformation($"Location: {reading.LocationID}");
            _logger.LogInformation($"Temp: {reading.Temperature}");

            return Content("OK");
        }
    }
}
