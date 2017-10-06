using Nsc.Wu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nsc.Wu.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
           /// DateTime d = DateTime.Parse("31-05-")
            string date = DateTime.Now.ToDateTime(Convert.ToInt64("1496188800000".Substring(0, 10))).ToShortDateString();
            return View();
        }
    }
}
