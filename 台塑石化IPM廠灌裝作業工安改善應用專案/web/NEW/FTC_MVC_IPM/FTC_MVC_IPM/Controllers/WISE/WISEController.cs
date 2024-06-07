using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FTC_MES_MVC.Controllers.WISE
{
    public class WISEController : Controller
    {
        // GET: WISE
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult WISEPattern()
        {
            return View();
        }
    }
}