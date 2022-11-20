using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlisTimer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlisTimer.Data;
using System.Net.Security;
using ConsoleApp1;
using Microsoft.EntityFrameworkCore;
using SimplicateAPI.ReturnTypes;

namespace BlisTimer.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private TimerDbContext _context;

        public LoginController(ILogger<LoginController> logger, TimerDbContext context)
        {
            _logger = logger;
            _context = context;

            // var h = new PHasher(new OptionsHash(1000));
            // var d = Guid.NewGuid().ToString();
            // var employee = new Employee()
            // {
            //     Id = d,
            //     Name = "Dirk-jan",
            //     LastName = "Kroon",
            //     Email = "dirk-jan@weareblis.com",
            //     Password = h.Hash("Hr2022$!S")
            // };
            // _context.Add(employee);
            // _context.SaveChanges();
        }

        public IActionResult Index()
        {
            var _ = new LoginForm();
            return View(_);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginForm emp)
        {

            var hasher = new PHasher(new OptionsHash(1000));

            var loginResult = await ApiDatabaseHandler.SimplicateApiClient.Login.TryUnsafeLoginAsync(emp.Email, emp.Password);
            
            //The login via the api was succesfull so we log them in, if the info is not yet in the database we add it.
            if (loginResult.IsSuccess)
            {
                var query = await _context.Employees.Where(_ => _.Email == emp.Email).ToListAsync();
                if (query.Count == 0)
                {
                    await _context.AddAsync( new Employee() {Id = loginResult.User.EmployeeId,Email = emp.Email,Password = hasher.Hash(emp.Password),Name = loginResult.User.FirstName,LastName = loginResult.User.FamilyName,Role = loginResult.User.IsAccountOwner ? 2 : 1,Projects = new List<Project>()});
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            
            //The login was not succesfull via the api, we check if the details are in the database if so, we log them in. Else we show an error. 
            else if (loginResult.Status == LoginResult.LoginStatus.BadCredentials)
            {
                var allHashedPasswords = await _context.Employees.Where(_ => _.Email == emp.Email).Select(_ => _.Password).ToListAsync();
            
                //loops trough all the hashed passwords that matches the input email
                //then checks if the hashed version of the given password matches any of the hashed once in the database
                foreach (var hashedPassword in allHashedPasswords) 
                {
                    var check = hasher.Check(hashedPassword, emp.Password);
                    if (check.verified)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                emp.Status = "BadCredentials";
                return View(emp);
            }
            return View(emp);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
