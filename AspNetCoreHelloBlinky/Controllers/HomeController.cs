using System.Diagnostics;
using AspNetCoreHelloBlinky.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreHelloBlinky.Controllers
{
    public class HomeController : Controller
    {
        private readonly LedBlinkClient _blinkClient;

        public HomeController(LedBlinkClient blinkClient)
        {
            _blinkClient = blinkClient;
        }

        public IActionResult Index()
        {
            ViewBag.BlinkState = _blinkClient.IsBlinking ? "Blinking" : "Not blinking";

            return View();
        }

        public IActionResult StartBlinking()
        {
            _blinkClient.StartBlinking();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult StopBlinking()
        {
            _blinkClient.StopBlinking();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
