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
            var projectDict = new Dictionary<Tuple<BlisTimer.Models.Project, bool>, List<WorkActivity>>();
                
            if (String.IsNullOrEmpty(projectId))
            {
                ViewBag.PreSelectedProject = false;
                foreach (var project in _context.Projects.Include(_ => _.Activities))
                {
                    projectDict.Add(Tuple.Create(project, false), project.Activities.ToList());
                }    
            }
            else
            {
                ViewBag.PreSelectedProject = true;
                var SelectedProject = _context.Projects.Include(_ => _.Activities).Where(_ => _.Id == projectId).FirstOrDefault();
                foreach (var project in _context.Projects.Include(_ => _.Activities))
                {
                    if (project.Id == SelectedProject.Id)
                    {
                        projectDict.Add(Tuple.Create(project, true), project.Activities.ToList());
                    }
                    else
                    {
                        projectDict.Add(Tuple.Create(project, false), project.Activities.ToList());
                    }
                }    

            }
            
            
            return View(projectDict);
        }
        [HttpPost]
        public IActionResult Index(string Id)
        {
            
            HttpContext.Session.SetString("ProjectId", Id);
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Index2(string Id)
        {
            
            HttpContext.Session.SetString("ActivityId", Id);
            
            return RedirectToAction("Index");
        }
    }
 }
