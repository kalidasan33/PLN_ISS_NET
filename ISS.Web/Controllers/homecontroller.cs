using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ISS.BusinessRules.Contract;
using ISS.Core.Model.Information;
using System.ComponentModel.DataAnnotations;

namespace ISS.Web.Controllers
{
    public class HomeController : BaseController
    {
         
        public ActionResult Index()
        {
         

            return View();
        }
         

        public ActionResult ISS()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Maintenance()
        {
           

            return View();
        }

        public ActionResult ComingSoon()
        {
           

            return View();
        }

       
    }

    
}
