using BlisTimer.Data;
using BlisTimer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project = SimplicateAPI.Enitities.Project;

namespace BlisTimer.Controllers
{
    public class TimerController : Controller
    {
        private TimerDbContext _context { get; set; }
        public TimerController(TimerDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var projectId = HttpContext.Session.GetString("ProjectId");
            var projectDict = new Dictionary<BlisTimer.Models.Project, List<WorkActivity>>();
                
            if (String.IsNullOrEmpty(projectId))
            {
                ViewBag.PreSelectedProject = true;
                foreach (var project in _context.Projects.Include(_ => _.Activities))
                {
                    projectDict[project] = new List<WorkActivity>();
                    foreach (var activity in project.Activities)
                    {
                        projectDict[project].Add(activity);
                    }
                }    
            }
            else
            {
                ViewBag.PreSelectedProject = false;
                var p = _context.Projects.Include(_ => _.Activities).Where(_ => _.Id == projectId).FirstOrDefault();
                projectDict[p] = new List<WorkActivity>();
                foreach (var activity in p.Activities)
                {
                    projectDict[p].Add(activity);
                    
                }    
            }
            
            
            return View(new SelectProject(){ ProjectDictionary = projectDict});
        }
        [HttpPost]
        public IActionResult Index(string Id)
        {
            
            HttpContext.Session.SetString("ProjectId", Id);
            
            return RedirectToAction("Index");
        }
    }
 }
