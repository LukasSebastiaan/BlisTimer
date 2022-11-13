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
        public EmployeeController(TimerDbContext context)
        {
            _context = context;
        }

        // public async Task<IActionResult> Index(){
        //     var projects = await _context.Projects.ToListAsync();
        //     return View(projects);
        // }

        // [HttpGet]
        // public IActionResult Add(){
        //     return View();
        // } 
        // [HttpPost]
        // public async Task<IActionResult> Add(ProjectAdd addProjectRequest){
        //     var project = new Project(){
        //         Id = Guid.NewGuid().ToString(),
        //         Name = addProjectRequest.Name
        //     };

            
        //     await _context.Projects.AddAsync(project);
        //     await _context.SaveChangesAsync();

        //     return RedirectToAction("Index");
        //} 
    }
}