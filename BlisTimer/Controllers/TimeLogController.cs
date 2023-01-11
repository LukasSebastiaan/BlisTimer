using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
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
        public async Task<IActionResult> Index() {
            var timeLogs = await _context.TimeLogs.Include(_ => _.Activity.Project).Include(_ => _.HourType).Where(_ => _.EmployeeId == HttpContext.User.Claims.ToList()[0].Value).ToListAsync();
            return View(timeLogs);
        }
        [Authorize]
        public IActionResult Edit(string id)
        {
            if (!_databasehandler.IsLoggedIn(HttpContext))
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.log = _context.TimeLogs.Where(_ => _.Id == id).Single();
            ViewBag.max = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
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
        public async Task<IActionResult> Add(TimeLogAdd addTimeLog){
            var d = Guid.NewGuid().ToString();
            var timelog = new TimeLog(){
                Id = d,
            };
            var p = _context.Employees.Where(_ => _.Name == addTimeLog.EmployeeName).Include(_ => _.EmployeeProjects).FirstOrDefault();
            timelog.EmployeeId = p.Id;
            timelog.Employee = p;
            var a = _context.WorkActivities.Where(_ => _.Id == "2c46d7b1-3481-4839-a97d-7a1dc21ee11c").Include(_ => _.Project).FirstOrDefault();
            timelog.ActivityId = a.Id;
            timelog.Activity = a;

            await _context.TimeLogs.AddAsync(timelog);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
            
            
        }
        
        [HttpPost]
        public async Task<IActionResult> SumbitTimelog(int timeModified)
        {
            var employeeId = HttpContext.User.Claims.ToList()[0].Value;
            
            if (!_context.RunningTimers.Any(_ => _.EmployeeId == employeeId))
                return BadRequest("There aren't any active timers running for the employee");

            var runningTimer = _context.RunningTimers.FirstOrDefault(_ => _.EmployeeId == employeeId);

            var Id = Guid.NewGuid().ToString();
            var StartTime = runningTimer.StartTime.AddHours(1).AddSeconds(timeModified * -1);
            var EndTime = DateTime.Now.ToUniversalTime().AddHours(1);
            var ActivityId = HttpContext.Session.GetString("ActivityId");
            var HourTypeId = HttpContext.Session.GetString("HourTypeId");
            var EmployeeId = employeeId;
            
            await _context.TimeLogs.AddAsync(new TimeLog()
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = runningTimer.StartTime.AddSeconds(timeModified * -1),
                EndTime = DateTime.Now.ToUniversalTime(),
                ActivityId = HttpContext.Session.GetString("ActivityId")!,
                HourTypeId = HttpContext.Session.GetString("HourTypeId")!,
                EmployeeId = employeeId!,
            });

            _context.RunningTimers.Remove(runningTimer);

            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddRunningTimer(int time)
        {
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
                var startTime = timer.StartTime.ToUniversalTime().AddHours(1);
                timer.StartTime = timer.StartTime.AddSeconds(time * -1);
                

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception err)
            {
                _logger.LogError("Couldn't add running timer to database with error: " + err.Message);
                return BadRequest("There was probably already a timer running for this employee");
            }
        }
    }
}