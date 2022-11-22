using Microsoft.AspNetCore.Mvc;

namespace BlisTimer.Controllers
{
    public class TimerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
