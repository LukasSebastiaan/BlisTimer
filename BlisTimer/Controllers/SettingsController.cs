using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace BlisTimer.Controllers
{
    public class SettingsController : Controller
    {
        private readonly TimerDbContext _context;
        private readonly ILogger<SettingsController> _logger;
        private readonly ApiDatabaseHandler _databasehandler;

        public SettingsController(TimerDbContext context, ILogger<SettingsController> logger, ApiDatabaseHandler databaseHandler)
        {
            _logger = logger;
            _context = context;
            _databasehandler = databaseHandler;
        }
        
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var preference = _context.Preferences.FirstOrDefault(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value);
            ViewBag.Preferences = preference;
            ViewBag.timeInHours = (preference.NotificationTimeSeconds/60)/60;
            ViewBag.checkBoxEnabled = preference.NotificationEnabled;
            ViewBag.ModifyCount = preference.ChangeCountTimeSeconds;
            return View();
        }
        
        [Authorize, HttpPost]
        public async Task<IActionResult> Index(PreferencesForm pForm)
        {
            var preference = _context.Preferences.FirstOrDefault(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value);
            preference.NotificationEnabled = pForm.NotificationEnabled ? 1 : 0;
            preference.NotificationTimeSeconds = pForm.NotificationTimeHours * 60 * 60;
            preference.ChangeCountTimeSeconds = pForm.ChangeCountTimeSeconds;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Settings");
        }
        
    }
}