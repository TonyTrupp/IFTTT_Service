using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IFTTT_App.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>Controller action for default homepage view</summary> 
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>Controller action for 500 error page</summary> 
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
