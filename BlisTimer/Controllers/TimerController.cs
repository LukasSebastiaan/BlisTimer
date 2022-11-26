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
                ViewBag.PreSelectedActivity = false;
                var SelectedProject = _context.Projects.Include(_ => _.Activities).Where(_ => _.Id == projectId).FirstOrDefault();
                foreach (var project in _context.Projects.Include(_ => _.Activities))
                {
                    if (project.Id == SelectedProject.Id)
                    {
                        projectDict.Add(Tuple.Create(project, true), project.Activities.ToList());
                        if (!String.IsNullOrEmpty(HttpContext.Session.GetString("ActivityId")))
                        {
                            ViewBag.PreSelectedActivityId = HttpContext.Session.GetString("ActivityId");
                            ViewBag.PreSelectedActivityName = HttpContext.Session.GetString("ActivityName");
                            ViewBag.PreSelectedActivity = true;
                        }
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
            HttpContext.Session.SetString("ActivityId", "");
            HttpContext.Session.SetString("ActivityName", "");
            
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Index2(string Id)
        {
            
            HttpContext.Session.SetString("ActivityId", Id);
            HttpContext.Session.SetString("ActivityName", _context.WorkActivities.Where(_ => _.Id == Id).Select(_ => _.Name).FirstOrDefault());
            
            return RedirectToAction("Index");
        }
        public IActionResult Index3(string Id)
        {
            
            HttpContext.Session.SetString("HourTypeId", Id);
            HttpContext.Session.SetString("HourtypeName", _context.HourTypes.Where(_ => _.HourTypeId == Id).Select(_ => _.Label).FirstOrDefault());
            
            return RedirectToAction("Index");
        }
    }
 }
