using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            // This is a catch-all controller that will return the index.html file from the client app
            // if the user tries to navigate to a route that doesn't exist in the API
            return PhysicalFile(
                Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "index.html"
                ), 
                "text/HTML"
            );
        }
    }
}