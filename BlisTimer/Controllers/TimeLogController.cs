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
                .Where(_ => (!_.Deleted && !_.Submitted)).OrderByDescending(_ => _.EndTime).ToList();

            var totalWorkedHours = timeLogs.Select(_ => _.EndTime - _.StartTime).Sum(_ => _.TotalHours);
            var time = TimeSpan.FromHours(totalWorkedHours);
            ViewBag.totalTime = $"{time.TotalHours.ToString("0#")}:{time.Minutes.ToString("0#")}:{time.Seconds.ToString("0#")}";

            return View(timeLogs);
        }

        
        [Authorize]
        public async Task<RedirectToActionResult> Delete(string id)
        {
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;

            TimeLog timelog;
            try {
                timelog = await _context.TimeLogs.Where(_ => _.Id == id && _.EmployeeId == employeeId).SingleAsync();
            }
            catch (Exception e) {
                _logger.LogError(e, "Error while deleting timelog");
                return RedirectToAction("Index");
            }
            
            timelog.Deleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;
            var log = await _context.TimeLogs.SingleAsync(_ => _.Id == id && _.EmployeeId == employeeId);

            try
            {
                ViewBag.log = log;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while editing timelog");
                return RedirectToAction("Index");
            }
            
            ViewBag.min = log.StartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            ViewBag.start = log.StartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            ViewBag.end = log.EndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            ViewBag.cet = log.EndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            ViewBag.max = log.EndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            
            
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(TimeLogAdd timeForm, string id)
        {
            if (timeForm.StartTime > timeForm.EndTime)
                return Redirect("/Login?error=999");
            
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;
            
            TimeLog log;
            try {
                log = await _context.TimeLogs.Where(_ => _.Id == id && _.EmployeeId == employeeId).SingleAsync();
            }
            catch (Exception e) {
                _logger.LogError("User tried to edit a time-log that does not belong to them or that does not exist");
                return Redirect("/TimeLog");
            }

            log.StartTime = timeForm.StartTime.ToUniversalTime();
            log.EndTime = timeForm.EndTime.ToUniversalTime();
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SumbitTimelog(int timeModified)
        {
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;
            
            if (!_context.RunningTimers.Any(_ => _.EmployeeId == employeeId))
                return BadRequest("There aren't any active timers running for the employee");

            var runningTimer = _context.RunningTimers.FirstOrDefault(_ => _.EmployeeId == employeeId);

            if (runningTimer!.StartTime.AddSeconds(timeModified * -1) > DateTime.Now.ToUniversalTime())
                return BadRequest("Timer cannot be modified to a negative number");

            await _context.TimeLogs.AddAsync(new TimeLog()
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = runningTimer.StartTime.AddSeconds(timeModified * -1),
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
        [Authorize]
        public async Task<IActionResult> AddRunningTimer(int time)
        {
            if (time < 0)
                return BadRequest("Timer cannot start with a negative number");
                
            if (_context.RunningTimers.Any(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value))
            {
                Console.WriteLine("There is already a running timer");
                return BadRequest("There is already a running timer for the employee");
            }
            
            try
            {
                await _context.RunningTimers.AddAsync(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    StartTime = DateTime.Now.ToUniversalTime().AddSeconds(time * -1),
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

        public async Task<IActionResult> ChangeRunningTimer(int time)
        {
            try
            {
                var employeeId = HttpContext.User.Claims.ToList()[0].Value;

                var timer = _context.RunningTimers.FirstOrDefault(_ => _.EmployeeId == employeeId);
                
                var startTime = timer.StartTime.ToUniversalTime();
                
                if (startTime.AddSeconds(time * -1) > DateTime.Now.ToUniversalTime())
                    return BadRequest("Timer cannot go to a negative number");
                
                // Time has to be converted to negative, because we want to subtract the added minutes from the start date
                timer.StartTime = startTime.AddSeconds(time * -1);
                

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
        [Authorize]
        public async Task<IActionResult> PostHoursToSimplicate()
        {
            if (HttpContext.Session.GetString("hoursBeingSubmitted") == "true")
                return BadRequest("There are already hours being submitted");
            
            var hoursToSubmit = _context.TimeLogs
                .Where(_ => !_.Submitted && !_.Deleted)
                .Include(_ => _.Activity)
                .Include(_ => _.HourType)
                .Include(_ => _.Employee)
                .ToList()
                .Where(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value!).ToList();
            
            if (hoursToSubmit.Count == 0)
                return BadRequest("There are no hours to submit");
            
            HttpContext.Session.SetString("hoursBeingSubmitted", "true");

            try
            {
                var failedSubmissions = await _databasehandler.SubmitHoursToSimplicate(hoursToSubmit);
                
                var successfullySubmittedHours = hoursToSubmit.Where(_ => !failedSubmissions.Contains(_)).ToList();
                
                foreach (var hour in successfullySubmittedHours)
                    hour.Submitted = true;
                
                await _context.SaveChangesAsync();

                if (failedSubmissions.Any())
                {
                    HttpContext.Session.SetString("hoursBeingSubmitted", "false");
                    return BadRequest(300);
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("hoursBeingSubmitted", "false");
                _logger.LogError("Error while submitting hours to simplicate: " + e.Message);
                return BadRequest("Something went wrong while connecting to the simplicate API");
            }
            
            HttpContext.Session.SetString("hoursBeingSubmitted", "false");

            return Ok();
        }
    }
}