using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Account;
using FacebookClone.Models.ViewModels.Profile;

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
            WallDTO wall=new WallDTO();
            wall.Id = userId;
            wall.Message = "";
            wall.DateEdited = DateTime.Now;
            db.Wall.Add(wall);
            db.SaveChanges();
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

            string userType = "guest";
            if (username.Equals(user))
                userType = "owner";
            ViewBag.UserType = userType;


            if (userType == "guest")
            {
                UserDTO u1 = db.Users.FirstOrDefault(x => x.Username.Equals(user));
                int id1 = u1.Id;

                UserDTO u2 = db.Users.FirstOrDefault(x => x.Username.Equals(username));
                int id2 = u2.Id;

                FriendDTO f1 = db.Friends.FirstOrDefault(x => x.User1.Equals(id1) && x.User2.Equals(id2));
                FriendDTO f2 = db.Friends.FirstOrDefault(x => x.User1.Equals(id2) && x.User2.Equals(id1));

                if (f1 == null && f2 == null)
                {
                    ViewBag.NotFriends = "True";
                }
                if (f1 != null)
                {
                    if (!f1.Active)
                        ViewBag.NotFriends = "Pending";
                }

                if (f2 != null)
                {
                    if (!f2.Active)
                        ViewBag.NotFriends = "Pending";
                }


            }
            var friendCount = db.Friends.Count(x => x.User2 == userDTO.Id && x.Active == false);
            if (friendCount > 0)
            {
                ViewBag.FrCount = friendCount;
            }


            //get friend count
            UserDTO uDto = db.Users.FirstOrDefault(x => x.Username.Equals(username));

            int usernameid = uDto.Id;

            var friendCount2 = db.Friends.Count(x => (x.User1 == usernameid || x.User2 == usernameid) && x.Active);
            ViewBag.FCount = friendCount2;

            
            //get message count
            var messageCount = db.Messages.Count(x => x.To == userDTO.Id && x.Read == false);
            ViewBag.MsgCount = messageCount;

            WallDTO wall = new WallDTO();
            ViewBag.WallMessage = db.Wall.FirstOrDefault(x => x.Id == userDTO.Id).Message;
            ViewBag.UserId = userDTO.Id;

            List<int> friendsId1 =
                db.Friends.Where(x => x.User1 == userDTO.Id && x.Active == true).ToArray().Select(x => x.User2).ToList();
            List<int> friendsId2 =
    db.Friends.Where(x => x.User2 == userDTO2.Id && x.Active == true).ToArray().Select(x => x.User1).ToList();
            List<int> allFriendsIds = friendsId1.Concat(friendsId2).ToList();

            List<WallVM> walls =
                db.Wall.Where(x => allFriendsIds.Contains(x.Id))
                    .ToArray()
                    .OrderByDescending(x => x.DateEdited)
                    .Select(x => new WallVM(x))
                    .ToList();

            ViewBag.Walls = walls;
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