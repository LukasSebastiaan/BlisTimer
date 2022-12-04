﻿using BlisTimer.Data;
using BlisTimer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project = SimplicateAPI.Enitities.Project;

namespace BlisTimer.Controllers
{
    public class TimerController : Controller
    {
        private readonly TimerDbContext _context;
        private readonly ApiDatabaseHandler _api;
        public TimerController(TimerDbContext context, ApiDatabaseHandler api)
        {
            _context = context;
            _api = api;
        }
        public async Task<IActionResult> Index()
        {
            await _api.SyncDbWithSimplicate();
            var projectDict = new Dictionary<Tuple<BlisTimer.Models.Project, bool>, List<WorkActivity>>();
            
            var running = _context.RunningTimers.Where(_ => _.EmployeeId == HttpContext.Session.GetString("Id")).FirstOrDefault();
            ViewBag.RunningTimer = "false";
            ViewBag.Time = 0;
            
            //When there is already a timer running it should select all the details from this timer. 
            if (running is not null)
            {
                
                var pId = _context.WorkActivities.Include(_ => _.Project).Where(_ => _.Id == running.ActivityId)
                    .Select(_ => _.Project.Id).FirstOrDefault();
                HttpContext.Session.SetString("ProjectId", pId);
                ViewBag.PreSelectedActivityName = _context.WorkActivities.Where(_ => _.Id == running.ActivityId).Select(_ => _.Name).FirstOrDefault();
                HttpContext.Session.SetString("ActivityId", running.ActivityId);
                HttpContext.Session.SetString("ActivityName", _context.WorkActivities.Where(_ => _.Id == running.ActivityId).Select(_ => _.Name).FirstOrDefault());
                HttpContext.Session.SetString("HourTypeId", running.HourTypeId);
                HttpContext.Session.SetString("HourTypeName", _context.HourTypes.Where(_ => _.HourTypeId == running.HourTypeId).Select(_ => _.Label).FirstOrDefault());

                ViewBag.RunningTimer = "true";
                var time = ((int)(DateTime.Now - running.StartTime).TotalSeconds) - 3600;
                ViewBag.Time = time;
            }

            var projectId = HttpContext.Session.GetString("ProjectId");
            if (String.IsNullOrEmpty(projectId))
            {
                ViewBag.PreSelectedProject = false;
                var projectsOfUser = _context.Projects.Include(_ => _.Activities).Include(_ => _.EmployeeProjects)
                    .Where(_ => _.EmployeeProjects
                        .Where(x => x.EmployeeId == HttpContext.Session.GetString("Id"))
                        .Select(p => p.ProjectId).Contains(_.Id)).ToList();
                foreach (var project in projectsOfUser)
                {
                    projectDict.Add(Tuple.Create(project, false), project.Activities.ToList());
                }    
            }
            else
            {
                ViewBag.PreSelectedProject = true;
                ViewBag.PreSelectedProjectName = _context.Projects.Where(_ => _.Id == projectId).Select(_ => _.Name).FirstOrDefault();;
                ViewBag.PreSelectedActivity = false;
                var SelectedProject = _context.Projects.Include(_ => _.Activities).Where(_ => _.Id == projectId).FirstOrDefault();
                var projectsOfUser = _context.Projects.Include(_ => _.Activities).Include(_ => _.EmployeeProjects)
                    .Where(_ => _.EmployeeProjects
                        .Where(x => x.EmployeeId == HttpContext.Session.GetString("Id"))
                        .Select(p => p.ProjectId).Contains(_.Id)).ToList();
                    
                foreach (var project in projectsOfUser)
                {
                    if (project.Id == SelectedProject.Id)
                    {
                        projectDict.Add(Tuple.Create(project, true), project.Activities.ToList());
                        if (!String.IsNullOrEmpty(HttpContext.Session.GetString("ActivityId")))
                        {
                            ViewBag.PreSelectedActivityId = HttpContext.Session.GetString("ActivityId");
                            ViewBag.PreSelectedActivityName = HttpContext.Session.GetString("ActivityName");
                            ViewBag.PreSelectedActivity = true;
                            
                            ViewBag.PreSelectedHourType = false;
                            var hourTypeOfUser = _context.HourTypes.Include(_ => _.WorkActivityHourTypes)
                                .Where(_ => _.WorkActivityHourTypes
                                    .Where(x => x.WorkActivityId == HttpContext.Session.GetString("ActivityId"))
                                    .Select(p => p.HourType.HourTypeId).Contains(_.HourTypeId)).ToList();
                            
                            var hourTypeList = new List<HourType>();
                            foreach (var hourtype in hourTypeOfUser)
                            {
                                hourTypeList.Add(hourtype);
                                if(hourtype.HourTypeId == (HttpContext.Session.GetString("HourTypeId")))
                                {
                                    if (!String.IsNullOrEmpty(HttpContext.Session.GetString("HourTypeId")))
                                    {
                                        ViewBag.PreSelectedHourTypeId = HttpContext.Session.GetString("HourTypeId");
                                        ViewBag.PreSelectedHourTypeName = HttpContext.Session.GetString("HourTypeName");
                                        ViewBag.PreSelectedHourType = true;
                                    }
                                }
                                
                            }

                            ViewBag.PreSelectedHourTypeList = hourTypeList;

                        }
                    }
                    else
                    {
                        projectDict.Add(Tuple.Create(project, false), project.Activities.ToList());
                    }
                }    

            }
            
            //This Code takes care of the timer that might be already running.
            // if (running is not null)
            // {
            //     ViewBag.PreSelectedActivity = true;
            //     ViewBag.PreSelectedActivityId = running.ActivityId;
            //     ViewBag.PreSelectedActivityName = _context.WorkActivities.Where(_ => _.Id == running.ActivityId).Select(_ => _.Name).FirstOrDefault();
            //     ViewBag.PreSelectedProject = true;
            //     ViewBag.PreSelectedProjectName = _context.Projects.Include(_ => _.Activities).Where(_ => _.Activities.Select(_ => _.Id).Contains(_.Id)).Select(_ => _.Name).FirstOrDefault();
            //     ViewBag.PreSelectedHourType = true;
            //     ViewBag.PreSelectedHourTypeId = HttpContext.Session.GetString("HourTypeId");
            //     ViewBag.PreSelectedHourTypeName = HttpContext.Session.GetString("HourTypeName");
            // }
            
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
            var p = _context.WorkActivities.Where(_ => _.Id == Id).Select(_ => _.Name).FirstOrDefault();
            HttpContext.Session.SetString("ActivityName", p);
            HttpContext.Session.SetString("HourTypeId", "");

            
            return RedirectToAction("Index");
        }
        public IActionResult Index3(string Id)
        {
            
            HttpContext.Session.SetString("HourTypeId", Id);
            HttpContext.Session.SetString("HourTypeName", _context.HourTypes.Where(_ => _.HourTypeId == Id).Select(_ => _.Label).FirstOrDefault());
            
            return RedirectToAction("Index");
        }
    }
 }
