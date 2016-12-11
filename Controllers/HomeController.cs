using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FreneticDocs.Models;

namespace FreneticDocs.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult ErrorInternal()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Error404()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Index()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Commands()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }
    }
}
