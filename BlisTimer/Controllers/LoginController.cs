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
using SimplicateAPI.ReturnTypes;

namespace BlisTimer.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var _ = new LoginForm();
            return View(_);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginForm emp)
        {
            var loginResult = await ApiDatabaseHandler.SimplicateApiClient.Login.TryUnsafeLoginAsync(emp.Email, emp.Password);
            if (loginResult.IsSuccess)
            {
                return RedirectToAction("Index","Home");
            }
            else if (loginResult.Status == LoginResult.LoginStatus.BadCredentials)
            {
                emp.Status = "BadCredentials";
                return View(emp);
            }
            return View(emp);
        }
        

        /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
    }
}
