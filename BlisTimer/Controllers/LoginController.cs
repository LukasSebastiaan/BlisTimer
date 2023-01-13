using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using BlisTimer.Models;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using BlisTimer.Data;
using ConsoleApp1;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
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
        public async Task<IActionResult> Index(int? error)
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Clear();
            }

            if (error.HasValue)
            {
                switch (error)
                {
                    case 501:
                        ViewBag.ErrorMessage = "There is something wrong the the Simplicate API. Please try again later. " +
                                                "If you have already logged in before, you can still use the site, but you will " +
                                                "be working with old or outdated data.";
                        break;
                    case 500:
                        ViewBag.ErrorMessage = "The website is currently down due to an error. Please try again later.";
                        break;
                    case 404:
                        ViewBag.ErrorMessage = "The page you are looking for does not exist.";
                        break;
                    default:
                        ViewBag.ErrorMessage = "An unknown error has occurred.";
                        break;
                }
            }
            
            return View(new LoginForm());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginForm emp)
        {
            if (!await _context.Database.CanConnectAsync())
              return Redirect("Login?error=500");
            
            _logger.LogInformation("Getting hasher to hash password");
            var hasher = new PHasher(new OptionsHash(1000));
            
            
            _logger.LogInformation("Trying to log user with in Simplicate with data:\n\t" + emp.Email + "\n\t" + emp.Password);
            var loginResult = await _apiDatabaseHandler.SimplicateApiClient.Login.TryUnsafeLoginAsync(emp.Email, emp.Password);
            
            _logger.LogInformation("Simplicate login returned: " + loginResult.Status);
            if (loginResult.IsSuccess)
            {
                var query = await _context.Employees.Where(_ => _.Id == loginResult.User.EmployeeId).ToListAsync();
                if (query.Count == 0)
                {
                    await _context.AddAsync(
                        new Preferences()
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = loginResult.User!.EmployeeId,
                            NotificationEnabled = 1, //1 == true, 0 == false
                            ChangeCountTimeSeconds = 15,
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
                else //We should check if they changed their password, if so their details are not correct in our database and need to be update
                {
                    var allHashedPasswords = await _context.Employees.Where(_ => _.Email == emp.Email).Select(_ => _.Password).ToListAsync();
                    
                    //loops trough all the hashed passwords that matches the input email
                    //then checks if the hashed version of the given password matches any of the hashed once in the database
                    bool isUpdated = false;
                    foreach (var hashedPassword in allHashedPasswords) 
                    {
                        var check = hasher.Check(hashedPassword, emp.Password);
                        if (check.verified)
                        {
                            isUpdated = true;
                            break;
                        }
                    }
                    if (!isUpdated)
                    {
                        _logger.LogInformation("Password was not up-to-date");
                        var employee = _context.Employees.Where(_ => _.Id == loginResult.User.EmployeeId).FirstOrDefault();
                        if (employee != null)
                        {
                            employee.Password = hasher.Hash(emp.Password);
                        }

                        await _context.SaveChangesAsync();
                    }
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

                var identity = new ClaimsIdentity(claims, "Cookies");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);


                await HttpContext.SignInAsync("Cookies", claimsPrincipal);
                
                await _apiDatabaseHandler.SyncDbWithSimplicate();
                
                return RedirectToAction("Index", "Timer");
            }
            
            //The login was not successful via the api, we check if the details are in the database if so, we log them in. Else we show an error. 
            if (loginResult.Status is LoginResult.LoginStatus.Failed or LoginResult.LoginStatus.ServerError)
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
            }
            
            if (loginResult.Status is LoginResult.LoginStatus.ServerError)
                return Redirect("Login?error=501");
            
            ViewBag.ErrorMessage = "The email or password you entered is incorrect.";
            return View(emp);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Console.WriteLine("ERROR LOLOLOLADSHUIDHASIFDHSDUIFHSDUIFHSDUIFG");
            return RedirectToAction("Index");
        }
    }
}