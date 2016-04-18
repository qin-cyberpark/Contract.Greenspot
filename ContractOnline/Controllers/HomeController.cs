using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ContractOnline.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Preview(ContractOnline.Models.FormViewModel model)
        {
            Session["CURRENT_APPLICATION"] = model;
            return Content("ok");
        }

        [HttpGet]
        public ActionResult Preview()
        {
            var model = (ContractOnline.Models.FormViewModel)Session["CURRENT_APPLICATION"];
            ViewBag.Model = model;
            return View();
        }

        [HttpPost]
        public ActionResult Submit()
        {
            var model = (ContractOnline.Models.FormViewModel)Session["CURRENT_APPLICATION"];
            if(model == null)
            {
                return null;
            }

            var filePath = ConfigurationManager.AppSettings["pdfFolder"] + model.FileName;
            model.ToPDF(filePath);
            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //return File(fileBytes, "application/pdf");
            if (Utilities.Send(model.Email, filePath))
            {
                return Redirect("/pdf/" + model.FileName);
            }
            else
            {
                return Content("Oops...something went wrong.");
            }
        }

        public ActionResult Terms()
        {
            return View();
        }
    }
}