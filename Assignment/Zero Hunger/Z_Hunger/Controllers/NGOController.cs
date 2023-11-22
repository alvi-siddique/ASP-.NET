using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Z_Hunger.Auth;
using Z_Hunger.EF;

namespace Z_Hunger.Controllers
{
    public class NGOController : Controller
    {
        [Logged]
        public ActionResult Index()
        {
            return View();
        }

        [Logged]
        [HttpGet]
        public ActionResult AddEmployee()
        {
            return View();
        }

        [Logged]
        [HttpPost]
        public ActionResult AddEmployee(Regestration r)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ZeroHungerEntities2())
                {
                    if (db.Regestrations.Any(e => e.Email == r.Email))
                    {
                        ModelState.AddModelError("Email", "This Email is already in use, please choose another email.");
                        return View(r);
                    }

                    if (r.Role == "admin")
                    {
                        // Create and add NGO entity
                        var ngoEntity = new NGO
                        {
                            Name = r.Name,
                            Email = r.Email,
                            Password = r.Password,
                            Role = r.Role
                        };
                        db.NGOs.Add(ngoEntity);
                    }
                    else if (r.Role == "employee")
                    {
                        // Create and add Employee entity
                        var employeeEntity = new Employee
                        {
                            Name = r.Name,
                            Email = r.Email,
                            Password = r.Password,
                            Role = r.Role
                        };
                        db.Employees.Add(employeeEntity);
                    }


                    // Always add the registration entity
                    db.Regestrations.Add(new Regestration { Name = r.Name, Email = r.Email, Password = r.Password, Role = r.Role });

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return View(r);
            }
        }

        [Logged]
        public ActionResult ViewRequest()
        {
            using (var db = new ZeroHungerEntities2())
            {
                var data = db.CollectionRequests.Where(cr => cr.Status != "Rejected").ToList();
                return View(data);
            }
        }

        [Logged]
        [HttpGet]
        public ActionResult RejectRequest(int id)
        {
            using (var db = new ZeroHungerEntities2())
            {
                var exStatus = db.CollectionRequests.FirstOrDefault(n => n.CollectionRequestID == id);
                return View(exStatus);
            }
        }

        [Logged]
        [HttpPost]
        public ActionResult RejectRequest(CollectionRequest cr)
        {
            using (var db = new ZeroHungerEntities2())
            {
                var exData = db.CollectionRequests.Find(cr.CollectionRequestID);
                exData.Status = "Rejected";
                db.SaveChanges();
                return RedirectToAction("ViewRequest");
            }
        }

        [Logged]
        public ActionResult RejectedRequest()
        {
            using (var db = new ZeroHungerEntities2())
            {
                var data = db.CollectionRequests.Where(cr => cr.Status == "Rejected").ToList();
                return View(data);
            }
        }

        [Logged]
        [HttpGet]
        public ActionResult AcceptRequest(int id)
        {
            using (var db = new ZeroHungerEntities2())
            {
                var ExData = db.CollectionRequests.FirstOrDefault(n => n.CollectionRequestID == id);
                return View(ExData);
            }
        }

        [Logged]
        [HttpPost]
        public ActionResult AcceptRequest(CollectionRequest cr)
        {
            using (var db = new ZeroHungerEntities2())
            {
                var exData = db.CollectionRequests.Find(cr.CollectionRequestID);
                exData.Status = "Processing";
                exData.EmployeeID = cr.EmployeeID;
                db.SaveChanges();
                return RedirectToAction("ViewRequest");
            }
        }

        [Logged]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
