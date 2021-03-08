using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iTellerBranch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        
        [HttpGet]
        public ActionResult TemplateSetup()
        {
            string uid = "1";
            var url = @"~/Cheque/Cheques.application?uid=" + uid +
               "&url=net.tcp://" + HttpContext.Request.Url.Host + ":" + "900/PayITService&opmode=" + 0 + "";
            return new RedirectResult(url);
        }

        [HttpGet]
        public ActionResult Print(long tranid)
        {
            string uid = "1";
            var url = @"~/Cheque/Cheques.application?uid=" + uid +
               "&url=net.tcp://" + HttpContext.Request.Url.Host + ":" + "900/PayITService&opmode=" + 1 + "&transCode=" + tranid + "";
            return new RedirectResult(url);
        }
    }
}
