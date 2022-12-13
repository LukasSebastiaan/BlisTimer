using System.Net;
using BlisTimer.Models;
using BlisTimer.Data;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlisTimer.Controllers
{
    public class TimeLogController : Controller
    {
        private readonly TimerDbContext _context;
        private readonly ILogger<TimeLogController> _logger;
        private readonly ApiDatabaseHandler _databasehandler;

        public TimeLogController(TimerDbContext context, ILogger<TimeLogController> logger, ApiDatabaseHandler databaseHandler)
        {
            _logger = logger;
            _context = context;
            _databasehandler = databaseHandler;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var timeLogs = _context.TimeLogs
                .Include(_ => _.Activity.Project)
                .Include(_ => _.HourType)
                .Where(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value)
                .ToList()
                .Where(_ => (!_.Deleted && !_.Submitted)).ToList();

            var totalWorkedHours = timeLogs.Select(_ => _.EndTime - _.StartTime).Sum(_ => _.TotalHours);
            var time = TimeSpan.FromHours(totalWorkedHours);
            ViewBag.totalTime = $"{time.TotalHours.ToString("0#")}:{time.Minutes.ToString("0#")}:{time.Seconds.ToString("0#")}";

            return View(timeLogs);
        }
        [Authorize]
        public IActionResult Edit(string id)
        {
            ViewBag.log = _context.TimeLogs.Where(_ => _.Id == id).Single();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TimeLogAdd timeForm, string id)
        {
            var log = await _context.TimeLogs.Where(_ => _.Id == id).SingleAsync();
            log.StartTime = DateTime.SpecifyKind(timeForm.StartTime, DateTimeKind.Utc);
            log.EndTime = DateTime.SpecifyKind(timeForm.EndTime, DateTimeKind.Utc);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> SumbitTimelog()
        {
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;
            
            if (!_context.RunningTimers.Any(_ => _.EmployeeId == employeeId))
                return BadRequest("There aren't any active timers running for the employee");

            var runningTimer = _context.RunningTimers.FirstOrDefault(_ => _.EmployeeId == employeeId);

            await _context.TimeLogs.AddAsync(new TimeLog()
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = runningTimer.StartTime,
                EndTime = DateTime.Now.ToUniversalTime(),
                ActivityId = HttpContext.Session.GetString("ActivityId")!,
                HourTypeId = HttpContext.Session.GetString("HourTypeId")!,
                ProjectId = HttpContext.Session.GetString("ProjectId")!,
                EmployeeId = employeeId!,
            });

            _context.RunningTimers.Remove(runningTimer);

            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddRunningTimer()
        {
            try
            {
                await _context.RunningTimers.AddAsync(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    StartTime = DateTime.Now.ToUniversalTime(),
                    EmployeeId = HttpContext.User.Claims.ToList()[0].Value!,
                    ActivityId = HttpContext.Session.GetString("ActivityId")!,
                    HourTypeId = HttpContext.Session.GetString("HourTypeId")!,
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception err)
            {
                _logger.LogError("Couldn't add running timer to database with error: " + err.Message);
                return BadRequest("There was probably already a timer running for this employee");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> PostHoursToSimplicate()
        {
            if (HttpContext.Session.GetString("hoursBeingSubmitted") == "true")
                return BadRequest("There are already hours being submitted");
            
            var hoursToSubmit = _context.TimeLogs
                .Where(_ => _.Submitted == false)
                .Include(_ => _.Activity)
                .Include(_ => _.HourType)
                .Include(_ => _.Employee)
                .ToList()
                .Where(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value!).ToList();
            
            if (hoursToSubmit.Count == 0)
                return BadRequest("There are no hours to submit");
            
            HttpContext.Session.SetString("hoursBeingSubmitted", "true");
            
            var failedSubmissions = await _databasehandler.SubmitHoursToSimplicate(hoursToSubmit);
            
            var successfullySubmittedHours = hoursToSubmit.Where(_ => !failedSubmissions.Contains(_)).ToList();
            
            foreach (var hour in successfullySubmittedHours)
            {
                hour.Submitted = true;
            }
            
            _context.SaveChanges();
            
            HttpContext.Session.SetString("hoursBeingSubmitted", "false");

            return Ok();
        }
    }
}