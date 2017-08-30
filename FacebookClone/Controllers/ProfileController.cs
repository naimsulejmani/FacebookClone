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
    }
}