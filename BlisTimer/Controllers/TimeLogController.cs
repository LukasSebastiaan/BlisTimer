using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class TimeLogController : Controller
    {
        private readonly TimerDbContext _context;
        private readonly ILogger<TimeLogController> _logger;

        public TimeLogController(TimerDbContext context,ILogger<TimeLogController> logger)
        {
            _logger = logger;
            _context = context;
        }            
        
        
        public async Task<IActionResult> Index(){
            var timeLogs = await _context.TimeLogs.Include(_ => _.Activity).Include(_ => _.Employee).ToListAsync();
            return View(timeLogs);
        }

        [HttpGet]
        public IActionResult Add(){
            return View();
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
        public async Task<IActionResult> SumbitTimelog()
        {
            var employeeId = HttpContext.Session.GetString("Id");
            
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
                    EmployeeId = HttpContext.Session.GetString("Id")!,
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
    }
}