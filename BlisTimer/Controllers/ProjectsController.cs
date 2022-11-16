using BlisTimer.Models;
using BlisTimer.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Controllers
{
    public class ProjectsController : Controller
    {
        private TimerDbContext _context;
        public ProjectsController(TimerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(){
            var projects = await _context.Projects.Include(_ => _.Activities).ToListAsync();
            return View(projects);
        }

        [HttpGet]
        public IActionResult Add(){
            return View();
        } 
        [HttpPost]
        public async Task<IActionResult> Add(ProjectAdd addProjectRequest){
            var d = Guid.NewGuid().ToString();
            var project = new Project(){
                Id = d,
                Name = addProjectRequest.Name
            };
            var p = _context.Projects.Where(_ => _.Id == "c5852ae9-4eae-4e98-ba19-94f64100c697").Include(_ => _.Activities).FirstOrDefault();
            var activ = new WorkActivity(){Id = Guid.NewGuid().ToString(), Name = "Frontend dev", ProjectId = d};
            var activ2 = new WorkActivity(){Id = Guid.NewGuid().ToString(), Name = "Backend dev", ProjectId = d};
            // project.Activities.Add();
            // project.Activities.Add(new Models.Activity(){Id = Guid.NewGuid().ToString(), Name = "Backend dev", ProjectId = d});
            // project.Activities.Add(new Models.Activity(){Id = Guid.NewGuid().ToString(), Name = "Testing", ProjectId = d});
            await _context.Projects.AddAsync(project);
            await _context.WorkActivities.AddAsync(activ);
            await _context.WorkActivities.AddAsync(activ2);

            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        } 
    }
}