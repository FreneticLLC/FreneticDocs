using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FreneticDocs.Models;

namespace FreneticDocs.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Reload()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }
    }
}
