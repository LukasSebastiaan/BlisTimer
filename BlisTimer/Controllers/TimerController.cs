using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class TimerController : Controller
    {
        private TimerDbContext _context { get; set; }
        public TimerController(TimerDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string id)
        {
            var activityId = HttpContext.Session.GetString("ActivityId");
            if (!String.IsNullOrEmpty(id))
            {
                HttpContext.Session.SetString("ActivityId", id);
                var activity = _context.WorkActivities.Include(_ => _.Project).Where(_ => _.Id == id).FirstOrDefault();
                return View(activity);
            }
            else if (!String.IsNullOrEmpty(activityId))
            {
                id = activityId;
                var activity = _context.WorkActivities.Include(_ => _.Project).Where(_ => _.Id == id).FirstOrDefault();
                return View(activity);
            }


            return RedirectToAction("Index", "Projects");
        }
    }
}
