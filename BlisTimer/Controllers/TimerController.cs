using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class TimerController : Controller
    {
        private readonly TimerDbContext _context;
        private readonly ApiDatabaseHandler _apiDatabaseHandler;

        public TimerController(TimerDbContext context, ApiDatabaseHandler apiDatabaseHandler)
        {
            _context = context;
            _apiDatabaseHandler = apiDatabaseHandler;
        }
        
        public async Task<IActionResult> Index(string id)
        {
            await _apiDatabaseHandler.SyncDbWithSimplicate();
            
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
