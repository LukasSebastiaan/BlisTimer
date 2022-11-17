using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class EmployeeController : Controller
    {
        private TimerDbContext _context;
        public EmployeeController(TimerDbContext context) =>
            _context = context;
        
        
        /*
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.Include(_ => _.Projects).ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Add(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeAdd addEmployee){
            var d = Guid.NewGuid().ToString();
            var employee = new Employee(){
                Id = d,
                Name = addEmployee.Name,
                LastName = addEmployee.LastName
            };
            
            await _context.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        } 
    */
    }
}