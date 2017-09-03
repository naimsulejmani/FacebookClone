using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Profile;

namespace FacebookClone.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            Db db = new Db();
            //create list of relevant search
            List<LiveSearchUserVM> usernameSearchUserVms =
                db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name)
                //    .Select(x => new LiveSearchUserVM(x))
                //    .ToList();
                    .ToArray()
                    .Select(x => new LiveSearchUserVM(x))
                    .ToList();

            //return json
            return Json(usernameSearchUserVms);
        }

        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
         //init db
            Db db = new Db();
            //get user id
            UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userId = userDTO.Id;
            UserDTO userDTO2 = db.Users.FirstOrDefault(x => x.Username.Equals(friend));
            int friendId = userDTO2.Id;
            

            FriendDTO friendDto = new FriendDTO();
            friendDto.User1 = userId;
            friendDto.User2 = friendId;
            friendDto.Active = false;
            db.Friends.Add(friendDto);
            db.SaveChanges();
            //get friend to be id
            //add DTO   
        }

        // POST: Profile/DisplayFriendRequests
        [HttpPost]
        public JsonResult DisplayFriendRequests()
        {
            Db db = new Db();

            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userid = userDto.Id;
            List<FriendRequestVM> list =
                db.Friends.Where(x => x.User2 == userid && x.Active == false)
                    .ToArray()
                    .Select(x => new FriendRequestVM(x))
                    .ToList();

            List<UserDTO> users = new List<UserDTO>();
            foreach (var friendRequestVm in list)
            {
                var user = db.Users.FirstOrDefault(x => x.Id == friendRequestVm.User1);
                users.Add(user);
            }

            return Json(users);
        }


        // POST: Profile/AcceptFriendRequests
        [HttpPost]
        public void AcceptFriendRequests(int friendId)
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userId = userDto.Id;

            FriendDTO friendDto = db.Friends.FirstOrDefault(x => x.User1 == friendId && x.User2 == userId);
            friendDto.Active = true;
            db.SaveChanges();

        }

        // POST: Profile/DeclineFriendRequests
        [HttpPost]
        public void DeclineFriendRequests(int friendId)
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userId = userDto.Id;

            FriendDTO friendDto = db.Friends.FirstOrDefault(x => x.User1 == friendId && x.User2 == userId);


            if (friendDto != null) db.Friends.Remove(friendDto);
            db.SaveChanges();

        }

        // POST: Profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userId = userDto.Id;

            UserDTO userDto2 = db.Users.FirstOrDefault(x => x.Username.Equals(friend));
            int userId2 = userDto2.Id;

            MessageDTO dto = new MessageDTO();
            dto.From = userId;
            dto.To = userId2;
            dto.Message = message;
            dto.DateSent = DateTime.Now;
            dto.Read = false;
            db.Messages.Add(dto);
            db.SaveChanges();

        }

        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            Db db = new Db();
            UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name));
            int userId = userDTO.Id;
            List<MessageVM> list =
                db.Messages.Where(x => x.To == userId && x.Read == false)
                    .ToArray()
                    .Select(x => new MessageVM(x))
                    .ToList();

            db.Messages.Where(x=>x.To==userId&& x.Read==false).ToList().ForEach(x=>x.Read=true);
            db.SaveChanges();

            return Json(list);
        }
        // POST: Profile/UpdateWallMessage
        [HttpPost]
        public void UpdateWallMessage(int id, string message)
        {
            Db db = new Db();
            WallDTO wall = db.Wall.Find(id);
            wall.Message = message;
            wall.DateEdited = DateTime.Now;
            db.SaveChanges();
        }
    }
}