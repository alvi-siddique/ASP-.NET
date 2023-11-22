using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Z_Hunger.Auth;
using Z_Hunger.EF;

namespace Z_Hunger.Controllers
{
    public class EmployeeController : Controller
    {
        [ELogged]
        public ActionResult Index()
        {
            int EmployeeID = (int)Session["EmployeeID"];

            using (var db = new ZeroHungerEntities2())
            {
                // Use concise LINQ query
                var data = db.CollectionRequests
                    .Where(e => e.EmployeeID == EmployeeID && e.Status != "Accepted")
                    .ToList();

                return View(data);
            }
        }

        [ELogged]
        [HttpGet]
        public ActionResult AcceptRequest(int id)
        {
            using (var db = new ZeroHungerEntities2())
            {
                // Use concise LINQ query
                var exData = db.CollectionRequests.FirstOrDefault(n => n.CollectionRequestID == id);

                return View(exData);
            }
        }

        [ELogged]
        [HttpPost]
        public ActionResult AcceptRequest(CollectionRequest cr)
        {
            using (var db = new ZeroHungerEntities2())
            {
                // Use concise LINQ query
                var exData = db.CollectionRequests.Find(cr.CollectionRequestID);

                if (exData != null)
                {
                    exData.Status = "Accepted";
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }

        [ELogged]
        public ActionResult CompletedRequest()
        {
            int EmployeeID = (int)Session["EmployeeID"];

            using (var db = new ZeroHungerEntities2())
            {
                // Use concise LINQ query
                var data = db.CollectionRequests
                    .Where(e => e.EmployeeID == EmployeeID && e.Status == "Accepted")
                    .ToList();

                return View(data);
            }
        }

        [ELogged]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
