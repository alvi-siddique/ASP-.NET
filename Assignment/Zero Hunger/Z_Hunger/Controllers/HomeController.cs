using System;
using System.Linq;
using System.Web.Mvc;
using Z_Hunger.Auth;
using Z_Hunger.EF;

namespace Z_Hunger.Controllers
{
    public class HomeController : Controller
    {
        [Logged]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            using (var db = new ZeroHungerEntities2())
            {
                var matchs = db.Regestrations.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (matchs != null)
                {
                    if (matchs.Role == "admin")
                    {
                        // Use LINQ to retrieve NGO ID
                        int NGOid = db.NGOs
                            .Where(n => n.Email == email)
                            .Select(n => n.NGOid)
                            .SingleOrDefault();

                        Session["Email"] = email;
                        Session["NGOid"] = NGOid;

                        return RedirectToAction("Index", "NGO");
                    }

                    else if (matchs.Role == "employee")
                    {
                        // Use LINQ to retrieve Employee ID
                        int EmployeeID = db.Employees
                            .Where(e => e.Email == email)
                            .Select(e => e.EmployeeID)
                            .SingleOrDefault();

                        Session["EmployeeEmail"] = email;
                        Session["EmployeeID"] = EmployeeID;

                        return RedirectToAction("Index", "Employee");
                    }

                    else if (matchs.Role == "restaurant")
                    {
                        // Use LINQ to retrieve Restaurant ID
                        int restaurantID = db.Restaurants
                            .Where(r => r.RestauranEmail == email)
                            .Select(r => r.RestaurantID)
                            .SingleOrDefault();

                        Session["RestaurantEmail"] = email;
                        Session["RestaurantID"] = restaurantID;

                        return RedirectToAction("Index", "Restaurant");
                    }

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            return View();
        }
    }
}
