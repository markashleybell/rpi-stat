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
    }
}
