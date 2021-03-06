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

        public IActionResult Tags()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Objects()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult TagBases()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Search()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Events()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }

        public IActionResult Explanations()
        {
            ViewData["init"] = new DocsInit(Request, Response);
            return View();
        }
    }
}
