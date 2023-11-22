using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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

        [Logged]
        [HttpPost]
        public ActionResult RegesterRestaurant(Regestration r)
        {
            if (ModelState.IsValid)
            {

                var db = new ZeroHungerEntities2();
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
                db.SaveChanges();

                db.Regestrations.Add(regestrationEntity);
                db.SaveChanges();

                return RedirectToAction("Index", "NGO");
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
            if (ModelState.IsValid)
            {

                var db = new ZeroHungerEntities2();

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


            else
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return View();
            }

        }


        [RLogged]
        public ActionResult CreatedRequest()
        {
            int RestaurantID = (int)Session["RestaurantID"];
            var db = new ZeroHungerEntities2();
            var data = db.CollectionRequests.Where(cr => cr.RestaurantID == RestaurantID).ToList();
            db.SaveChanges();
            return View(data);
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
            var db = new ZeroHungerEntities2();
            var existingRequest = db.CollectionRequests.Find(id);

            if (existingRequest == null || existingRequest.RestaurantID != RestaurantID)
            {
                // If the request is not found or doesn't belong to the restaurant, handle accordingly
                return HttpNotFound();
            }

            return View(existingRequest);
        }

        [RLogged]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CollectionRequest editedRequest)
        {
            int RestaurantID = (int)Session["RestaurantID"];

            if (ModelState.IsValid)
            {
                var db = new ZeroHungerEntities2();
                var existingRequest = db.CollectionRequests.Find(editedRequest.CollectionRequestID);

                if (existingRequest == null || existingRequest.RestaurantID != RestaurantID)
                {
                    // If the request is not found or doesn't belong to the restaurant, handle accordingly
                    return HttpNotFound();
                }

                // Update the properties of the existing request
                existingRequest.IteamName = editedRequest.IteamName;
                existingRequest.ExpiredTime = editedRequest.ExpiredTime;

                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("CreatedRequest");
            }

            // If ModelState is not valid, return to the edit view with the model
            return View(editedRequest);
        }







        [RLogged]
        public ActionResult Delete(int id)
        {
            int RestaurantID = (int)Session["RestaurantID"];
            var db = new ZeroHungerEntities2();
            var exdata = db.CollectionRequests.Find(id);
            db.CollectionRequests.Remove(exdata);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}