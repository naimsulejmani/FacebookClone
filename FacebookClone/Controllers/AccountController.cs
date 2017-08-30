using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Account;

namespace FacebookClone.Controllers
{
    public class AccountController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            //Confirm user is not logged in

            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
            {
                return Redirect("~/" + username);
            }
            //return view here finally
            return View();
        }

        // POST: Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {
            Db db = new Db();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (db.Users.Any(x => x.Username.Equals(model.Username)))
            {
                ModelState.AddModelError("","Username "+model.Username+" is taken ");
                model.Username = "";
                return View("Index", model);
            }

            UserDTO userDTO = new UserDTO()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress,
                Password = model.Password,
                Username = model.Username
            };

            db.Users.Add(userDTO);
            db.SaveChanges();

            int userId = userDTO.Id;

            FormsAuthentication.SetAuthCookie(model.Username, false);

            var uploadsDir = new DirectoryInfo($"{Server.MapPath(@"\")}Uploads");
             
            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/gif" &&
                    ext != "image/jpg" &&
                    ext != "image/png")
                {
                       ModelState.AddModelError("","The image was not iuploaded wrong image extension.");
                return View("Index", model);
                }

                string imageName = userId + ".jpg";
                var pathName = uploadsDir.ToString() + @"/" + imageName;
                //var path = string.Format(@"{0)\\{1}", uploadsDir, imageName);
                file.SaveAs(pathName);
            }
            //init db
            //check model state
            //make sure username is unique
            //create userdto
            //add to dto
            //save
            //get inserted id
            //login to user
            //set uploads dir
            //check if a file was uploaded
            //get the extension
            //verify extensiuon
            //set image name
            //set image path
            //save image finally
            //redirect
            //return RedirectToAction("Username", new {username = model.Username});
            return Redirect("~/" + model.Username);
        }


        // GET: /{username}
        [Authorize]
        public ActionResult Username(string username = "")
        {

            Db db = new Db();

            //check if user exitsts
            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/"); 
            }

            ViewBag.Username = username;

            string user = User.Identity.Name;

            UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username.Equals(user));
            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;


            UserDTO userDTO2 = db.Users.FirstOrDefault(x => x.Username.Equals(username));
            ViewBag.ViewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;
            ViewBag.UsernameImage = userDTO2.Id + ".jpg";
            return View();
        }
        [Authorize]
        //GET: account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }

        public ActionResult LoginPartial()
        {
            return PartialView();
        }

        // POST: account/Login
        [HttpPost]
        public string Login(string username, string password)
        {
            //init db
            Db db = new Db();
            //chec if exists
            if (db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                FormsAuthentication.SetAuthCookie(username,false);
                return "ok";
            }
            return "problem";
            //log in
        }
    }
}