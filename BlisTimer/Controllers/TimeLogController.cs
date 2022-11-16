using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class TimeLogController : Controller
    {
        private TimerDbContext _context;
        public TimeLogController(TimerDbContext context)
        {
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
                AmountOfTimeSpentInSeconds = addTimeLog.AmountOfTimeSpentInSeconds
            };
            var p = _context.Employees.Where(_ => _.Name == addTimeLog.EmployeeName).Include(_ => _.Projects).FirstOrDefault();
            timelog.EmployeeId = p.Id;
            timelog.Employee = p;
            var a = _context.WorkActivities.Where(_ => _.Id == "2c46d7b1-3481-4839-a97d-7a1dc21ee11c").Include(_ => _.Project).FirstOrDefault();
            timelog.ActivityId = a.Id;
            timelog.Activity = a;

            // project.Activities.Add();
            // project.Activities.Add(new Models.Activity(){Id = Guid.NewGuid().ToString(), Name = "Backend dev", ProjectId = d});
            // project.Activities.Add(new Models.Activity(){Id = Guid.NewGuid().ToString(), Name = "Testing", ProjectId = d});
            await _context.TimeLogs.AddAsync(timelog);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        } 
    }
}