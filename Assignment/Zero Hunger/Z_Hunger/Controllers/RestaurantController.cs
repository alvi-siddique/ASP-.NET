using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Z_Hunger.Auth;
using Z_Hunger.EF;

namespace Z_Hunger.Controllers
{
    public class RestaurantController : Controller
    {
        [RLogged]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegesterRestaurant()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegesterRestaurant(Regestration r)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ZeroHungerEntities2())
                {
                    if (db.Regestrations.Any(s => s.Email == r.Email))
                    {
                        ModelState.AddModelError("RestauranEmail", "This Email already used, try another Email");
                        return View(r);
                    }

                    var restaurantEntity = new Restaurant
                    {
                        Name = r.Name,
                        RestauranEmail = r.Email,
                        Password = r.Password,
                        ConfirmPass = r.Password
                    };

                    var regestrationEntity = new Regestration
                    {
                        Name = r.Name,
                        Email = r.Email,
                        Password = r.Password,
                        Role = r.Role
                    };

                    db.Restaurants.Add(restaurantEntity);
                    db.Regestrations.Add(regestrationEntity);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
            }

            return View(r);
        }

        [RLogged]
        [HttpGet]
        public ActionResult CreateRequest()
        {
            return View();
        }

        [RLogged]
        [HttpPost]
        public ActionResult CreateRequest(string IteamName, string ExpiredTime, string status)
        {
            int RestaurantID = (int)Session["RestaurantID"];

            // Check if any of the required fields are null or empty
            if (string.IsNullOrEmpty(IteamName) || string.IsNullOrEmpty(ExpiredTime) || string.IsNullOrEmpty(status))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return View();
            }

            if (ModelState.IsValid)
            {
                using (var db = new ZeroHungerEntities2())
                {
                    var cr = new CollectionRequest
                    {
                        IteamName = IteamName,
                        CreationTime = DateTime.Now.ToString(),
                        ExpiredTime = ExpiredTime,
                        RestaurantID = RestaurantID,
                        Status = "Requesting"
                    };

                    db.CollectionRequests.Add(cr);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View();
            }
        }


        [RLogged]
        public ActionResult CreatedRequest()
        {
            int RestaurantID = (int)Session["RestaurantID"];
            using (var db = new ZeroHungerEntities2())
            {
                var data = db.CollectionRequests.Where(cr => cr.RestaurantID == RestaurantID).ToList();
                return View(data);
            }
        }

        [RLogged]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        [RLogged]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            int RestaurantID = (int)Session["RestaurantID"];
            using (var db = new ZeroHungerEntities2())
            {
                var existingRequest = db.CollectionRequests
                    .FirstOrDefault(cr => cr.CollectionRequestID == id && cr.RestaurantID == RestaurantID);

                if (existingRequest == null)
                {
                    return HttpNotFound();
                }

                return View(existingRequest);
            }
        }

        [RLogged]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CollectionRequest editedRequest)
        {
            int RestaurantID = (int)Session["RestaurantID"];

            if (ModelState.IsValid)
            {
                using (var db = new ZeroHungerEntities2())
                {
                    var existingRequest = db.CollectionRequests
                        .FirstOrDefault(cr => cr.CollectionRequestID == editedRequest.CollectionRequestID && cr.RestaurantID == RestaurantID);

                    if (existingRequest != null)
                    {
                        existingRequest.IteamName = editedRequest.IteamName;
                        existingRequest.ExpiredTime = editedRequest.ExpiredTime;

                        db.SaveChanges();
                    }

                    return RedirectToAction("CreatedRequest");
                }
            }

            return View(editedRequest);
        }

        [RLogged]
        public ActionResult Delete(int id)
        {
            int RestaurantID = (int)Session["RestaurantID"];
            using (var db = new ZeroHungerEntities2())
            {
                var exdata = db.CollectionRequests
                    .FirstOrDefault(cr => cr.CollectionRequestID == id && cr.RestaurantID == RestaurantID);

                if (exdata != null)
                {
                    db.CollectionRequests.Remove(exdata);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }
    }
}
