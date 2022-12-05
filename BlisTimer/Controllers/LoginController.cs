using System.Diagnostics;
using System.Security.Claims;
using System.Net;
using BlisTimer.Models;
using Microsoft.AspNetCore.Mvc;
using BlisTimer.Data;
using ConsoleApp1;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimplicateAPI.ReturnTypes;

namespace BlisTimer.Controllers
{
    
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly TimerDbContext _context;
        private readonly ApiDatabaseHandler _apiDatabaseHandler;

        public LoginController(ILogger<LoginController> logger, TimerDbContext context, ApiDatabaseHandler apiDatabaseHandler)
        {
            _logger = logger;
            _context = context;
            _apiDatabaseHandler = apiDatabaseHandler;
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Clear();
            }
            
            var updateDatabaseTask = _apiDatabaseHandler.SyncDbWithSimplicate();
            return View(new LoginForm());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginForm emp)
        {
            _logger.LogInformation("Getting hasher to hash password");
            var hasher = new PHasher(new OptionsHash(1000));
            
            _logger.LogInformation("Trying to log user with in Simplicate");
            var loginResult = await _apiDatabaseHandler.SimplicateApiClient.Login.TryUnsafeLoginAsync(emp.Email, emp.Password);
            
            //The login via the api was succesfull so we log them in, if the info is not yet in the database we add it.
            if (loginResult.IsSuccess)
            {
                
                var query = await _context.Employees.Where(_ => _.Email == emp.Email).ToListAsync();
                if (query.Count == 0)
                {
                    await _context.AddAsync(
                        new Preferences()
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = loginResult.User!.EmployeeId,
                            NotificationEnabled = true,
                            NotificationTimeSeconds = 3600,
                        }
                    );
                    await _context.AddAsync( new Employee()
                    {
                        Id = loginResult.User!.EmployeeId,
                        Email = emp.Email,
                        Password = hasher.Hash(emp.Password),
                        Name = loginResult.User.FirstName,
                        LastName = loginResult.User.FamilyName,
                        Role = loginResult.User.IsAccountOwner ? 2 : 1,
                    });
                    await _context.SaveChangesAsync();
                }
                
                HttpContext.Session.SetString("Firstname", loginResult.User!.FirstName);
                HttpContext.Session.SetString("Lastname", loginResult.User!.FamilyName);
                HttpContext.Session.SetString("Username", loginResult.User!.Username);
                HttpContext.Session.SetString("Email", loginResult.User!.Email);
                HttpContext.Session.SetString("Id", loginResult.User!.EmployeeId);

                var claims = new List<Claim>
                {
                    new Claim("Id", loginResult.User.EmployeeId),
                    new Claim("Username", loginResult.User.Username)
                };
                HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "Id", "Username")));
                
                await _apiDatabaseHandler.SyncDbWithSimplicate();
                
                return RedirectToAction("Index", "Timer");
                return RedirectToAction("Index", "Timer");
            }
            
            //The login was not successful via the api, we check if the details are in the database if so, we log them in. Else we show an error. 
            if (loginResult.Status == LoginResult.LoginStatus.Failed)
            {
                var allHashedPasswords = await _context.Employees.Where(_ => _.Email == emp.Email).Select(_ => _.Password).ToListAsync();
            
                //loops trough all the hashed passwords that matches the input email
                //then checks if the hashed version of the given password matches any of the hashed once in the database
                foreach (var hashedPassword in allHashedPasswords) 
                {
                    var check = hasher.Check(hashedPassword, emp.Password);
                    if (check.verified)
                    {
                        return RedirectToAction("Index", "Timer");
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
